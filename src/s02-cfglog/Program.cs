/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

using Azos;
using Azos.Apps;
using Azos.IO.Console;
using Azos.Log;
using Azos.Serialization.JSON;

namespace s02_cfglog
{
  public class Program
  {
    //Process entry point
    public static int Main(string[] args)
    {
      try
      {
        //Allocate app chassis, the chassis has a default logger
        //The aopp.Log gets built from `/log` config section automatically
        using (var app = new AzosApplication(args))
        {
          ////Just as an example, you can programmatically (with code) add a
          ////Console logger with coloring. A sink is a `Daemon`: you can Start/Stop daemons,
          ////so we need to call a `Start()` so it takes effect. We add the sink under App.Log:
          //new ConsoleSink(app.Log as ISinkOwner, "con", 0) { Colored = true }.Start();

          log_direct_info(app);
          log_direct_correlation(app);
          log_direct_error(app);

          log_component(app);

          return 0;
        }
      }
      catch (Exception error)//in case of whatever error leaking - we log it
      {
        //we can get here if the app chassis is badly configured and could not start
        ConsoleUtils.Error("Unhandled exception:");//with colors!
        ConsoleUtils.Warning(error.ToMessageWithType());
        return -1;//returning error code to the caller (e.g. bash scripting), so the caller can decide how to proceed
      }
    }


    private static void log_direct_info(IApplication app)
    {
      //Since this is a demo app we are going to demonstrate how to log things into logger directly
      //without using any framework support/component logging/DI, then gradually increase the complexity

      //Directly write to log - this method is rarely used (if ever) and only shown here for example.
      //Notice: the logger is ASYNC by design. You do not need to AWAIT the `Write()` as it always instantly returns.
      app.Log.Write(
        new Message{
          Type = MessageType.InfoA,// Info message
          Topic = "examples",        // Archetype of message topic - a grouper for structured log analysis, e.g. "db", "logic", "web"
          From = nameof(log_direct_info), // A name of component/code point, e.g. "LogicA.Method1"
          Text = "My sample log message text" // Free form message text, we will look at message parameters later
        }
      );
    }

    private static void log_direct_correlation(IApplication app)
    {
      //We are going to group multiple log messages into a conversation
      //Lets generate a relation tag
      var rel = Guid.NewGuid();

      app.Log.Write(
        new Message
        {
          RelatedTo = rel,

          Type = MessageType.InfoA,
          Topic = "examples",
          From = nameof(log_direct_correlation),
          Text = "Yo, this is my first message",
        }
      );

      app.Log.Write(
        new Message
        {
          RelatedTo = rel,

          Type = MessageType.Warning,
          Topic = "examples",
          From = nameof(log_direct_correlation),
          Text = "I am going to relate this warning to the first info",
        }
      );

      app.Log.Write(
        new Message
        {
          RelatedTo = rel,

          Type = MessageType.DebugC,
          Topic = "examples",
          From = nameof(log_direct_correlation),
          Text = "This is some Debug msg also correlated to the first two",
        }
      );
    }

    private static void log_direct_error(IApplication app)
    {
      //Now, we will log a synthetic error case to show other fields used for structured logging
      try
      {
        throw new Azos.Data.DocValidationException(//throw a fake data validation error
            schemaName: "medical.doctor", //Canonical schema name of data object
            message: "Doctor state is not valid: this provider not licensed in the requested medical procedure context"
        );
      }
      catch(Exception error)
      {
        app.Log.Write(
          new Message
          {
            Type = MessageType.Error,// Error message
            Topic = "examples", // Archetype of message topic - a grouper for structured log analysis, e.g. "db", "logic", "web"
            From = nameof(log_direct_error), // A name of component/code point, e.g. "LogicA.Method1"
            Source = 100_234, //add source tracepoint - this can simplify complex logging scenarios
            Text = "Bad doctor: " + error.ToMessageWithType(), // Just a text with exception type name

            //Add Exception graph, some sinks know how to unwind all of the details and generate alerts etc..
            //This is very much needed in Sky.Chronicle APM where you can browse exception details in a distributed
            //node cluster later
            Exception = error,

            //you can attach any string content for structured "parameter bag", you typically would use JSON
            //The parameters are typically just stored in the APM, and can be later indexed for later analysis
            Parameters = new {doc = "Smith Gregg", patient = "Ananda Sonali"}.ToJson(),

            //you can add archive dimensions = data keys for future log message analysis
            //we are adding "npi" provider id, "hcpc" medical procedure code, and financial class of the patient
            //Azos.Sky.Cronicle APM cloud solution automatically indexes log messages on the log archive dimensions
            ArchiveDimensions = ArchiveConventions.EncodeArchiveDimensions(new{npi = "1230623234", hcpc = "h204.2", fc = "pvt"})
          }.InitDefaultFields(app)
        );
      }
    }

    private static void log_component(IApplication app)
    {
      //in real life app you would sue Dependency Injection
      //we manually allocate a concrete component type for simplicity
      using(var cmp = new MyDemoComponent(app))
      {
        cmp.Demonstrate();
      }
    }
  }
}
