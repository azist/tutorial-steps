/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

using Azos;
using Azos.Apps;
using Azos.Log;

namespace s02_cfglog
{

  /// <summary>
  /// Here is a demo component. A component is a part of Azos application - a little building block - a box in an app container.
  /// Azos framework manages components for you: injects, configures, patches dependencies etc..
  /// In This example we are creating a silly component that does no real service but demos its logging capabilities.
  /// </summary>
  public class MyDemoComponent : ApplicationComponent
  {
    public MyDemoComponent(IApplication app) : base(app)
    {
      //If the type is Trace, then it is not going to be logged because by default component log level is Info and up
      WriteLogFromHere(MessageType.Info, "I am in constructor");
    }

    protected override void Destructor()
    {
      WriteLogFromHere(MessageType.Info, "I am in destructor");
      base.Destructor();
    }

    //we set the topic for all logging performed via WriteLog*()
    public override string ComponentLogTopic => "examples";

    /// <summary>
    /// Performs demo by logging messages
    /// </summary>
    public void Demonstrate()
    {
      var rel = WriteLogFromHere(MessageType.Info, "Well, you kind of saw already how component logging works");

      WriteLog(MessageType.Info, nameof(Demonstrate), "The WriteLog allows you to pass `from`", related: rel);
      WriteLog(MessageType.Info, nameof(Demonstrate), $"The log level in effect is: {ComponentEffectiveLogLevel}", related: rel);
      WriteLog(MessageType.Info, nameof(Demonstrate), $"The log level of this component is: {ComponentLogLevel}", related: rel);
      WriteLog(MessageType.Info, nameof(Demonstrate), $"If this component does not specify log level of its own, then it takes it from the parent entity", related: rel);

      //Encloses an action in try catch and logs the error if it leaked from action.
      //This method never leaks. Returns true if there was no error on action success,
      //or false if error leaked from action and was logged by component. The actual
      //logging depends on component log level
      var success = this.DontLeak(() => throw new Exception("This is dangerous!!!"), "Dangerous action crashed!!!");

      Aver.IsFalse(success, "success is false because DontLeak() swallowed an error");
    }
  }
}
