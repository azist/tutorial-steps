/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Text;

using Azos;
using Azos.Serialization.JSON;
using Azos.Wave.Mvc;

using Tutorial.Security.Permissions;
using Tutorial.Web.Controllers;

namespace Tutorial.Server.Web.SysControllers
{
  [NoCache]
  public sealed class Test : TutorialOAuthController
  {
    /// <summary>
    /// Echoes back what was supplied. Binds to any HTTP verb.
    /// </summary>
    [Action]
    public object Echo(JsonDataMap got)
      => new { OK = true, got};

    /// <summary>
    /// Echoes back what was supplied. The caller needs to be authenticated and authorized as `ApiCaller`
    /// </summary>
    [Action, ApiCaller]
    public object SecureEcho(JsonDataMap got)
      => new { OK = true, got, who = Ambient.CurrentCallUser.Name };

    /// <summary>
    /// Throws an exception with optional `text`. This is used to test error filter setup
    /// </summary>
    [Action, ApiCaller]
    public object Throw(string text)
      => throw new TutorialException($"This was thrown: {text.Default("<none>")}");


    /// <summary>
    /// Logs an error without throwing it
    /// </summary>
    [Action, ApiCaller]
    public object LogError(string topic, string text)
    {
      var rel = Guid.NewGuid();

      App.Log.Write(new Azos.Log.Message
      {
        Guid = rel,
        Type = Azos.Log.MessageType.Error,
        Topic = topic.Default("Unspecified"),
        From = nameof(Test),
        Text = text.Default("Unspecified"),
        Exception = new TutorialException("Exception message"),
        ArchiveDimensions = Azos.Log.ArchiveConventions.EncodeArchiveDimensions(new { topic, text })
      });

      return new {OK = true, id = rel};
    }

    [Action, ApiCaller]
    public object Convert(decimal pct, DateTime dob)
     => new {
        effectiveOptions = Data.DataConversion.EffectiveOptions,
        original = new { pct, dob },
        convterted = new
        {
          pct = Data.DataConversion.ConvertIntegerPercentageFromAPI(pct),
          dob = Data.DataConversion.RecodeDOBasUTC(dob)
        }
     };
  }
}
