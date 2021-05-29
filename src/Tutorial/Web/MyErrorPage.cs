/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Text;

using Azos;
using Azos.Data;
using Azos.Platform;
using Azos.Wave;

namespace Tutorial.Web
{
  ///<summary>
  /// An example of custom error page
  ///</summary>
  public class MyErrorPage : Azos.Wave.Templatization.WaveTemplate
  {
    public MyErrorPage(Exception error, bool showDump)
    {
      Error = error;
      ShowDump = showDump;
    }

    public readonly Exception Error;
    public bool ShowDump;

    protected override void DoRender()
    {
      var content = GetType().GetText("Error.html");

      content = content.Replace("{{ID}}", Context.ID.ToString())
                       .Replace("{{APP}}", Context.App.Name)
                       .Replace("{{DESCRIPTION}}", Context.App.Description)
                       .Replace("{{STATUS}}", "{0} {1}".Args(Context.Response.StatusCode, Context.Response.StatusDescription))
                       .Replace("{{TIME}}", Ambient.UTCNow.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture))
                       .Replace("{{COPYRIGHT}}", Context.App.Copyright);

      if (/*ShowDump &&*/ Error!=null)
      {
        var details = dumpError(Error);
        content = content.Replace("{{DETAILS}}", details);
      }
      else
      {
        content = content.Replace("{{DETAILS}}", "Tracing id: {0}".Args(Context.ID));
      }

      Target.Write(content);
    }

    private string dumpError(Exception error)
    {
      var output = new StringBuilder();
      dumpError(output, error, 1);
      return output.ToString();
    }

    private void dumpError(StringBuilder output, Exception error, int level)
    {
      if (error is FilterPipelineException fpe) error = fpe.RootException;

      output.AppendLine("<h3> {0}. {1}</h3>".Args(level, Target.Encode(error.GetType().FullName)));
      output.AppendLine("<div class=\"code\">{0}</div>".Args(Target.Encode(error.Message).AsString()));
      output.AppendLine("<h4> Stack: </h4>");
      output.AppendLine("<div class=\"code\"><ul>{0}</ul></div>".Args(Target.Encode(error.StackTrace).AsString().Replace("at ", "<li> at ")));
      output.AppendLine("<hr>");

      if (error.InnerException != null)
        dumpError(output, error.InnerException, level + 1);
    }

  }//class
}//namespace
