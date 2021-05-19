/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

using Azos;
using Azos.Apps;
using Azos.Conf;
using Azos.IO.Console;
using Azos.Platform;

namespace s01_console
{
  public class Program
  {
    //Process entry point
    public static int Main(string[] args)
    {
      try
      {
        //This entry point is .Net Core specific
        new Azos.Platform.Abstraction.NetCore.NetCore20Runtime();

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
      //if help was requested then show it and exit
      if (app.CommandArgs["?", "h", "help"].Exists)//if any of these switches passed
      {
        ConsoleUtils.WriteMarkupContent(typeof(Program).GetText("Help.txt")); //read markup from embedded resource
        return 0;
      }

      //The [] accessor coalesces sub-section names from left to right
      //until a section with such name is found. If nothing found then
      //a special "Nonexistent" sentinel node section is returned
      var isSilent = app.CommandArgs["s", "silent"].Exists; // `-s` or `-silent` switch in the root

      if (!isSilent)
      {
        ConsoleUtils.WriteMarkupContent(typeof(Program).GetText("Welcome.txt")); //read markup from embedded resource
        ConsoleUtils.Info($"Id: {app.AppId}");
        ConsoleUtils.Info($"Name: {app.Name}");
        ConsoleUtils.Info($"Description: {app.Description}");
        ConsoleUtils.Warning($"Environment: {app.EnvironmentName}");
        ConsoleUtils.Info($"Here is how your command arguments look: \n\n{app.CommandArgs.ToLaconicString()}\n\n");
        ConsoleUtils.Info($"Started at: {app.TimeSource.UTCNow} UTC");
      }

      //Get a reference to `-work` command line switch which contains
      // attributes, e.g. a type looks like `-work type="........"` this way
      //we can inject object types implementing the `Work` contract
      var workConf = app.CommandArgs[Work.CONFIG_WORK_SECTION];

      //Polymorphism: inject dependency using `type=FQN` syntax
      var work = FactoryUtils.MakeAndConfigure<Work>(workConf, typeof(DefaultWork));//if `type` was not supplied, use `DefaultWork`

      //Polymorphism: what is done depends on the actual type of work object
      var result = work.Perform(); //<---- PERFORM ACTUAL WORK

      if (!isSilent)
      {
        ConsoleUtils.Info($"Finished at: {app.TimeSource.UTCNow} UTC");
      }

      return result;
    }
  }
}
