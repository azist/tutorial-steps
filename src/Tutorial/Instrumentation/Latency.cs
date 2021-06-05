/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

using Azos.Instrumentation;
using Azos.Serialization.Bix;

namespace Tutorial.Instrumentation
{
  /// <summary>
  /// Measures data query execution times
  /// </summary>
  [Serializable, Bix("5A9BB7A1-7470-46C5-835B-47293D431CCE")]
  public sealed class DataQueryLatency : TutorialLongGauge, INetInstrument, IDatabaseInstrument
  {
    /// <summary>
    /// Emits the latency reading
    /// </summary>
    public static void Emit(IInstrumentation inst, string ctxName, string queryName, long msLatency, string queryParam = null)
    {
      if (inst == null || !inst.Enabled) return;
      var src = $"{ctxName}::{queryName}::{queryParam}";
      var datum = new DataQueryLatency(src, msLatency);
      inst.Record(datum);

      datum = new DataQueryLatency(UNSPECIFIED_SOURCE, msLatency);//general
      inst.Record(datum);
    }

    internal DataQueryLatency(string src, long value) : base(src, value) { }

    public override string Description => "Measures data query execution time";
    public override string ValueUnitName => Azos.CoreConsts.UNIT_NAME_MSEC;

    protected override Datum MakeAggregateInstance() => new DataQueryLatency(Source, 0);
  }

  /// <summary>
  /// Measures API execution times
  /// </summary>
  [Serializable, Bix("C7B32D5F-62D2-42B8-919C-737BBDB0DE38")]
  public sealed class ApiLatency : TutorialLongGauge, INetInstrument, IWebInstrument
  {
    /// <summary>
    /// Emits the Api latency reading and ApiCallEvent
    /// </summary>
    public static void EmitApiCall(IInstrumentation inst, string ctxName, string uri, long msLatency, bool emitEvent = true)
    {
      if (inst == null || !inst.Enabled) return;
      var src = $"{ctxName}::{uri}";
      var datum = new ApiLatency(src, msLatency);
      inst.Record(datum);

      datum = new ApiLatency(UNSPECIFIED_SOURCE, msLatency);//general
      inst.Record(datum);

      if (emitEvent)
      {
        inst.Record(new ApiCallEvent(src));
        inst.Record(new ApiCallEvent(UNSPECIFIED_SOURCE));
      }
    }

    internal ApiLatency(string src, long value) : base(src, value) { }

    public override string Description => "Measures API execution times";
    public override string ValueUnitName => Azos.CoreConsts.UNIT_NAME_MSEC;

    protected override Datum MakeAggregateInstance() => new ApiLatency(Source, 0);
  }

  /// <summary>
  /// Indicates an API call occurrence
  /// </summary>
  [Serializable, Bix("B8EAD801-69BB-4BDD-A88B-A218D1C49E42")]
  public sealed class ApiCallEvent : TutorialEvent
  {
    internal ApiCallEvent(string src) : base(src) { }

    public override string Description => "Indicates an API call occurrence";
    public override string ValueUnitName => Azos.CoreConsts.UNIT_NAME_TIME;

    protected override Datum MakeAggregateInstance() => new ApiCallEvent(Source);
  }

}

