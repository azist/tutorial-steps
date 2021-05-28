/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

using Azos.Conf;
using Azos.Platform;

namespace Tutorial.Data
{
  /// <summary>
  /// Provides methods for converting decimals, percentages and Dates according to expectations
  /// specified as `DataConversion.Options`
  /// </summary>
  public static class DataConversion
  {
    public const Options.DateHandling DEFAULT_DATES = Options.DateHandling.UTC;
    public const Options.PercentHandling DEFAULT_PERCENTAGES = Options.PercentHandling.Coefficients;
    public const bool DEFAULT_UTCDOB = true;


    public sealed class Options
    {
      public enum DateHandling { UTC = 0, AsIs }
      public enum PercentHandling { Coefficients = 0, AsIs, Int100, Int10000 }

      [Config] public DateHandling? Dates { get; set; }
      [Config] public PercentHandling? Percentages { get; set; }
      [Config] public bool? UTCDOB { get; set; }

      public void Reset()
      {
        Dates = null;
        Percentages = null;
        UTCDOB = null;
      }

      public void Configure(IConfigSectionNode cfg) => ConfigAttribute.Apply(this, cfg);
    }


    public static readonly Options Default = new Options
    {
      Dates = Options.DateHandling.UTC,
      Percentages = Options.PercentHandling.Coefficients,
      UTCDOB = true
    };

    private static AsyncFlowMutableLocal<Options> ats_EffectiveOptions = new AsyncFlowMutableLocal<Options>();

    public static Options EffectiveOptions => ats_EffectiveOptions.Value ?? (ats_EffectiveOptions.Value = new Options());


    /// <summary>
    /// Re-codes DateTimeKind.Utc on an existing date time value without making any time conversions.
    /// The time is set to 0:0:0
    /// </summary>
    public static DateTime? RecodeDOBasUTC(DateTime? existing)
    {
      if (existing == null) return null;
      var utcdob = EffectiveOptions.UTCDOB ?? Default.UTCDOB ?? DEFAULT_UTCDOB;
      if (!utcdob) return existing;

      return new DateTime(existing.Value.Year, existing.Value.Month, existing.Value.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    /// <summary>
    /// Converts integer value into percentage value as specified by API conversion expectation
    /// </summary>
    public static decimal? ConvertIntegerPercentageToAPI(int? existing)
    {
      if (existing == null) return null;
      var pcts = EffectiveOptions.Percentages ?? Default.Percentages ?? DEFAULT_PERCENTAGES;
      switch (pcts)
      {
        case Options.PercentHandling.Coefficients: return existing / 100m;
        case Options.PercentHandling.Int100: return (decimal)existing;
        case Options.PercentHandling.Int10000: return existing * 100m;
        default: return existing;
      }
    }

    /// <summary>
    /// Converts API value into system integer value
    /// </summary>
    public static int? ConvertIntegerPercentageFromAPI(decimal? existing)
    {
      if (existing == null) return null;
      var pcts = EffectiveOptions.Percentages ?? Default.Percentages ?? DEFAULT_PERCENTAGES;
      switch (pcts)
      {
        case Options.PercentHandling.Coefficients: return (int)(existing * 100m);
        case Options.PercentHandling.Int100: return (int)existing;
        case Options.PercentHandling.Int10000: return (int)(existing / 100m);
        default: return (int)existing;
      }
    }

  }
}

