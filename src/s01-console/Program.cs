/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

using Azos;
using Azos.Apps;
using Azos.IO.Console;
using Azos.Platform;

namespace s01_console
{
  class Program
  {
    //Process entry point
    private static int Main(string[] args)
    {
      try
      {
        //Allocate app chassis, passing command arguments
        //app configuration will be read by the chassis from the config file co-located
        //with the entry-point executable.
        //You can also use `-config file-name` command-line
        //parameter to use a different configuration file e.g. `dotnet s01-console.dll -config myapps.json`
        using (var app = new AzosApplication(args))
        {
          return main(app);
        }
      }
      catch(Exception error)//in case of whatever error leaking - we log it
      {
        //we can get here if the app chassis is badly configured and could not start
        ConsoleUtils.Error("Unhandled exception:");//with colors!
        ConsoleUtils.Warning(error.ToMessageWithType());
        return -1;//returning error code to the caller (e.g. bash scripting), so the caller can decide how to proceed
      }
    }

    //The core of application execution. The command args are already parsed
    //The app chassis is OK
    private static int main(IApplication app)
    {
      ConsoleUtils.WriteMarkupContent(typeof(Program).GetText("Welcome.txt")); //read markup from embedded resource

      ConsoleUtils.Info($"Id: {app.AppId}");
      ConsoleUtils.Info($"Name: {app.Name}");
      ConsoleUtils.Info($"Description: {app.Description}");
      ConsoleUtils.Warning($"Environment: {app.EnvironmentName}");


      return 0;
    }
  }
}
