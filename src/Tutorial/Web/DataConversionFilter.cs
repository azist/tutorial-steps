/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/


using System;
using System.Collections.Generic;

using Azos;
using Azos.Conf;
using Azos.Wave;

namespace Tutorial.Web
{
  /// <summary>
  /// Handles data conversion header by setting values on `Tutorial.Data.DataConversion.EffectiveOptions` ambient context.
  /// The default header name is `wv-data-convert`. If it is omitted then the process-global defaults are used
  /// </summary>
  /// <example>
  /// Header format example:
  /// <code>
  ///  ...
  ///  Accept: application/json
  ///  wv-data-conv: dates=utc percentages=asis utcdob=true
  ///  ...
  /// </code>
  /// The content of the header is the Laconic config vector applied to `DataConversion.Options` class.
  /// See <see cref="Data.DataConversion.Options"/> for configuration options
  /// </example>
  public sealed class DataConversionFilter : WorkFilter
  {
    public const string WV_DATA_CONV_HDR = "wv-data-convert";

    public DataConversionFilter(WorkDispatcher dispatcher, string name, int order) : base(dispatcher, name, order) { }
    public DataConversionFilter(WorkDispatcher dispatcher, IConfigSectionNode confNode) : base(dispatcher, confNode) { ConfigAttribute.Apply(this, confNode); }
    public DataConversionFilter(WorkHandler handler, string name, int order) : base(handler, name, order) { }
    public DataConversionFilter(WorkHandler handler, IConfigSectionNode confNode) : base(handler, confNode) { ConfigAttribute.Apply(this, confNode); }


    /// <summary>
    /// When set, reads the named header and sets DataConversion.EffectiveOptions
    /// </summary>
    [Config(Default = WV_DATA_CONV_HDR)]
    public string DataConversionHeader { get; set; } = WV_DATA_CONV_HDR;

    protected override void DoFilterWork(WorkContext work, IList<WorkFilter> filters, int thisFilterIndex)
    {
      Tutorial.Data.DataConversion.EffectiveOptions.Reset();

      //try to inject DataConversionHeader
      var dch = DataConversionHeader;
      if (dch.IsNotNullOrWhiteSpace())
        try
        {
          var hcontent = work.Request.Headers[dch];
          if (hcontent.IsNotNullOrWhiteSpace())
          {
            var hdr = hcontent.AsLaconicConfig(handling: Azos.Data.ConvertErrorHandling.Throw);
            Tutorial.Data.DataConversion.EffectiveOptions.Configure(hdr);
          }
        }
        catch (Exception error)
        {
          throw new HTTPStatusException(WebConsts.STATUS_406, WebConsts.STATUS_406_DESCRIPTION + ": " + dch, "Header error", error);
        }

      try
      {
        InvokeNextWorker(work, filters, thisFilterIndex);
      }
      finally
      {
        Tutorial.Data.DataConversion.EffectiveOptions.Reset();
      }
    }

  }
}
