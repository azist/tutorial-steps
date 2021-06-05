/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;

using Azos;
using Azos.Conf;
using Azos.Time;
using Azos.Wave;

namespace Tutorial.Web
{
  /// <summary>
  /// Measures request processing latency via ApiLatency/ms gauge and optionally emits ApiCallEvent
  /// </summary>
  public sealed class LatencyInstrumentationFilter : WorkFilter
  {
    public LatencyInstrumentationFilter(WorkDispatcher dispatcher, string name, int order) : base(dispatcher, name, order) { }
    public LatencyInstrumentationFilter(WorkDispatcher dispatcher, IConfigSectionNode confNode) : base(dispatcher, confNode) { ConfigAttribute.Apply(this, confNode); }
    public LatencyInstrumentationFilter(WorkHandler handler, string name, int order) : base(handler, name, order) { }
    public LatencyInstrumentationFilter(WorkHandler handler, IConfigSectionNode confNode) : base(handler, confNode) { ConfigAttribute.Apply(this, confNode); }


    /// <summary>
    /// When set, emits API call event instrument
    /// </summary>
    [Config(Default = true)]
    public bool EmitApiCallEvent { get; set; }

    protected override void DoFilterWork(WorkContext work, IList<WorkFilter> filters, int thisFilterIndex)
    {
      var ie = App.Instrumentation.Enabled;
      Timeter? time = null;
      if (ie) time = Timeter.StartNew();
      try
      {
        InvokeNextWorker(work, filters, thisFilterIndex);
      }
      finally
      {
        if (ie)
        {
          var uri = work.Request.Url.AbsolutePath;
          var ctx = work.Session?.DataContextName;
          Instrumentation.ApiLatency.EmitApiCall(App.Instrumentation, uri, ctx, time.Value.ElapsedMs, EmitApiCallEvent);
        }
      }
    }

  }
}

