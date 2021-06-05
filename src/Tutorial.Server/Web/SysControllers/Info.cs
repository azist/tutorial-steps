/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Azos;
using Azos.Apps.Injection;
using Azos.Instrumentation;
using Azos.Log;
using Azos.Log.Sinks;
using Azos.Platform;
using Azos.Platform.Instrumentation;
using Azos.Serialization.JSON;
using Azos.Wave;
using Azos.Wave.Mvc;

using Tutorial.Instrumentation;
using Tutorial.Security.Permissions;

namespace Tutorial.Server.Web.SystemControllers
{
  /// <summary>
  /// Provides system information overview
  /// </summary>
  [NoCache]
  [ApiControllerDoc(
    BaseUri = "/info",
    Title = "System Information",
    Description = @"Provides system operation status and information"
  )]
  public class Info : Controller
  {

    [Action]
    [ApiEndpointDoc(
      Uri = "index",
      Title = "Public Software build information",
      Description = "Returns public Software build information"
    )]
    public object Index() => new
    {
      OK = true,
      asof = App.TimeSource.UTCNow,
      env = App.EnvironmentName,
      ver = new
      {
        seed = Globals.THIS_BUILD_INFO.BuildSeed,
        date = Globals.THIS_BUILD_INFO.DateStampUTC
      }
    };

    [Action, ApiCaller(ApiAccessScope.Service)]
    [ApiEndpointDoc(
      Uri = "system",
      Title = "System details",
      Description = "Returns system details. Requires enterprise privileges"
    )]
    public object System() => new
    {
      OK = true,
      asof = App.TimeSource.UTCNow,
      host = Azos.Platform.Computer.HostName,
      appid = App.AppId,
      appname = App.Name,
      appstart = App.StartTime,
      instance = App.InstanceId,
      env = App.EnvironmentName,
      istest = App.IsUnitTest,
      last_error = App.Log.LastError,
      last_warning = App.Log.LastWarning,
      last_catastrophe = App.Log.LastCatastrophe,
      ver = new
      {
        azos = Azos.Conf.BuildInformation.ForFramework,
        this_system = Globals.THIS_BUILD_INFO
      }
    };

    [Action, ApiCaller(ApiAccessScope.Service)]
    [ApiEndpointDoc(
      Uri = "config",
      Title = "Effective process config tree",
      Description = "Returns effective process config tree in the dev/local environments"
    )]
    public object Config()
    {
      var cfg = App.ConfigRoot;
      var isDev = App.IsDeveloperEnvironment();
      if (!isDev) cfg = null;//the non-DEV and non-LOCAL environments are cloaked
      return new { OK = true, root = cfg };
    }


    [Inject] private ILog m_Log;

    private string getLogDiskPath()
    {
      var logger = m_Log as ISinkOwner;
      if (logger == null) return null;

      var fsink = logger.Sinks["gen"] as FileSink;

      if (fsink == null) fsink = logger.Sinks.OfType<FileSink>().FirstOrDefault();
      if (fsink == null) return null;

      return fsink.Path;
    }

    private MemoryBufferSink getMemoryBuffer()
    {
      var logger = m_Log as ISinkOwner;
      if (logger == null) return null;

      var sink = logger.Sinks["memory"] as MemoryBufferSink;
      return sink;
    }

    [ActionOnGet(Name = "log-list"), ApiCaller]
    [ApiEndpointDoc(
      Uri = "log-list",
      Title = "Effective process config tree",
      Description = "Returns effective process config tree in the dev/local environments"
    )]
    public void GetLogs()
    {
      var path = getLogDiskPath();
      if (path == null) throw HTTPStatusException.BadRequest_400("Could not determine local log path");

      var files = path.AllFileNames(false)
                      .Select(f => new FileInfo(f))
                      .Select(i => new { i.Name, i.Length, i.CreationTimeUtc, i.LastAccessTimeUtc, i.LastWriteTimeUtc });

      if (WorkContext.RequestedJson)
      {
        WorkContext.Response.WriteJSON(
          new
          {
            m_Log.LastWarning,
            m_Log.LastError,
            m_Log.LastCatastrophe,
            files
          });
        return;
      }

      var html = new StringBuilder();

      html.AppendLine("<html>");
      html.AppendLine($"<head>");
      html.AppendLine("<style> body{ font-family: 'Courier New'} td{padding: 6px;} </style>");
      html.AppendLine($" <title>Log Files on `{Azos.Platform.Computer.HostName}` at {App.TimeSource.UTCNow} UTC </title>");
      html.AppendLine("</head>");
      html.AppendLine("<body>");
      html.AppendLine("<table>");
      foreach (var f in files)
      {
        html.AppendLine("<tr>");
        html.AppendLine($"<td><a href='/info/log-file?fn={Uri.EscapeUriString(f.Name)}'>{f.Name}</a></td>");
        html.AppendLine("<td>{0:n0}</td>".Args(f.Length));
        html.AppendLine($"<td>{f.Length}</td>");
        html.AppendLine($"<td>{f.CreationTimeUtc}</td>");
        html.AppendLine($"<td>{f.LastAccessTimeUtc}</td>");
        html.AppendLine($"<td>{f.LastWriteTimeUtc}</td>");
        html.AppendLine("</tr>");
      }

      html.AppendLine("</table>");
      html.AppendLine("</body>");
      html.AppendLine("</html>");

      WorkContext.Response.ContentType = Azos.Web.ContentType.HTML;
      WorkContext.Response.Write(html.ToString());
    }

    [ActionOnGet(Name = "log-file"), ApiCaller]
    [ApiEndpointDoc(
      Uri = "log-list",
      Title = "Gets log file by name",
      Description = "gets log file by name with optional `attach` header"
    )]
    public object GetLogFile(string fn, bool attach = false)
    {
      var path = getLogDiskPath();
      if (path == null) throw HTTPStatusException.BadRequest_400("Could not determine local log path");
      var fullPath = Path.Combine(path, fn.NonBlank(nameof(fn).Replace("..", string.Empty)));

      using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      using (var rdr = new StreamReader(fs))
        return rdr.ReadToEnd();
    }

    [ActionOnGet(Name = "log-buffer"), ApiCaller]
    [ApiEndpointDoc(
      Uri = "log-buffer",
      Title = "Gets log buffer",
      Description = "Gets log buffer, asc=false by default, you can pass Accept header to get JSON instead of HTML"
    )]
    public void GetLogBuffer(bool asc = false)
    {
      var sink = getMemoryBuffer();
      if (sink == null) throw new TutorialException("You need to configure log sink of MemoryBufferSink type named `memory`");
      var data = asc ? sink.BufferedTimeAscending : sink.BufferedTimeDescending;

      if (WorkContext.RequestedJson)
      {
        WorkContext.Response.ContentType = Azos.Web.ContentType.JSON;
        WorkContext.Response.Write(new { OK = true, data }.ToJson(JsonWritingOptions.CompactRowsAsMap));
        return;
      }

      var html = new StringBuilder();

      html.AppendLine("<html>");
      html.AppendLine($"<head>");
      html.AppendLine("<style> body{ font-family: 'Courier New'; font-size: 10px;} table{ font-size: 11px;} td{padding: 4px;} </style>");
      html.AppendLine($" <title>Log buffer on `{Azos.Platform.Computer.HostName}` at {App.TimeSource.UTCNow} UTC </title>");
      html.AppendLine("</head>");
      html.AppendLine("<body>");
      html.AppendLine("<table>");
      foreach (var m in data)
      {
        html.AppendLine("<tr>");
        html.AppendLine($"<td>{m.Guid.ToString().TakeLastChars(6)}</td>");
        html.AppendLine("<td>{0:yyyyMMdd-HH:mm:ss:fff}</td>".Args(m.UTCTimeStamp));
        html.AppendLine($"<td>{m.Type}</td>");
        html.AppendLine($"<td>{WebUtility.HtmlEncode(m.Topic)}</td>");
        html.AppendLine($"<td>{WebUtility.HtmlEncode(m.From)}</td>");
        html.AppendLine($"<td>{WebUtility.HtmlEncode(m.Text)}</td>");
        html.AppendLine("</tr>");
      }

      html.AppendLine("</table>");
      html.AppendLine("</body>");
      html.AppendLine("</html>");

      WorkContext.Response.ContentType = Azos.Web.ContentType.HTML;
      WorkContext.Response.Write(html.ToString());
    }

    [ActionOnGet(Name = "dashboard"), ApiCaller]
    [ApiEndpointDoc(
    Uri = "dashboard",
    Title = "Gets dashboard UI driver",
    Description = "Gets Html page containing UI which visualizes instrumentation data using `dashboard-data` endpoint"
    )]
    public void GetDashboard()
    {
      WorkContext.Response.ContentType = Azos.Web.ContentType.HTML;
      WorkContext.Response.Write(GetType().GetText("api_dashboard.html").Replace("{{NAME}}", "{0} [{1}]".Args(App.AppId, App.EnvironmentName)));
    }

    [ActionOnGet(Name = "dashboard-data"), ApiCaller]
    [ApiEndpointDoc(
     Uri = "dashboard-data",
     Title = "Gets dashboard data snapshot",
     Description = "Gets dashboard data snapshot as of lastFetchStamp date"
     )]
    public object GetDashboardData(long lastFetchStamp = 0)
    {
      const int BATCH_SIZE = 512;

      var instr = App.Instrumentation;
      if (!instr.Enabled) return new { Ok = false, asof = App.TimeSource.UTCNow };

      var data = instr.GetBufferedResultsSince(new DateTime(lastFetchStamp, DateTimeKind.Utc))
                      .OrderBy(d => d.StartUtc)
                      .Take(BATCH_SIZE)//batch size limit
                      .ToArray();//make a snapshot

      object map(Datum d) => new { t = d.GetType().Name, at = d.StartUtc, cnt = d.Count, v = d.PlotValue };

      var cpu = data.Where(i => i is CPUUsage || i is RAMUsage)
                    .Select(d => map(d));

      var latency = data.Where(i => (i is ApiLatency || i is DataQueryLatency) && i.IsUnspecifiedSource)
                        .Select(d => map(d));

      var calls = data.Where(i => i is ApiCallEvent && i.IsUnspecifiedSource)
                      .Select(d => map(d));

      var stamp = data.Length > 0 ? data[data.Length - 1].EndUtc.Ticks + 1 : lastFetchStamp;

      return new
      {
        Ok = true,
        asof = App.TimeSource.UTCNow,
        cpu,
        latency,
        calls,
        lastFetchStamp = stamp
      };
    }
  }
}


