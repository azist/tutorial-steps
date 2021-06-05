/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

using Azos.Financial;
using Azos.Instrumentation;

namespace Tutorial.Instrumentation
{
  /// <summary>
  /// Marker interface for all instruments related to Tutorial system.
  /// </summary>
  public interface ITutorialInstrument
  {
  }

  /// <summary>
  /// A base gauge for making LONG scalar measurements
  /// </summary>
  [Serializable]
  public abstract class TutorialLongGauge : LongGauge, ITutorialInstrument
  {
    protected TutorialLongGauge(string src, long value) : base(src, value) { }
    protected TutorialLongGauge(string src, long value, DateTime utcDateTime) : base(src, value, utcDateTime) { }
  }

  /// <summary>
  /// A base gauge for making DOUBLE scalar measurements. Warning: do NOT store monetary values in this gauge,
  /// use either Amount or Decimal gauges instead
  /// </summary>
  [Serializable]
  public abstract class TutorialDoubleGauge : DoubleGauge, ITutorialInstrument
  {
    protected TutorialDoubleGauge(string src, double value) : base(src, value) { }
    protected TutorialDoubleGauge(string src, double value, DateTime utcDateTime) : base(src, value, utcDateTime) { }
  }

  /// <summary>
  /// A base gauge for making monetary measurements
  /// </summary>
  [Serializable]
  public abstract class TutorialDecimalGauge : DecimalGauge, ITutorialInstrument
  {
    protected TutorialDecimalGauge(string src, decimal value) : base(src, value) { }
    protected TutorialDecimalGauge(string src, decimal value, DateTime utcDateTime) : base(src, value, utcDateTime) { }
  }


  /// <summary>
  /// A base gauge for making Monetary Amount with currency measurements
  /// </summary>
  [Serializable]
  public abstract class TutorialAmountGauge : AmountGauge, ITutorialInstrument
  {
    protected TutorialAmountGauge(string src, Amount value) : base(src, value) { }
    protected TutorialAmountGauge(string src, Amount value, DateTime utcDateTime) : base(src, value, utcDateTime) { }
  }

  /// <summary>
  /// A base event for making custom events in your system
  /// </summary>
  [Serializable]
  public abstract class TutorialEvent : Event, ITutorialInstrument
  {
    protected TutorialEvent(string src) : base(src) { }
    protected TutorialEvent(string src, DateTime utcDateTime) : base(src, utcDateTime) { }
  }
}
