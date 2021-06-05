/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/


using System;
using System.Collections.Generic;
using System.Text;

using Azos;
using Azos.Platform;
using Azos.Security;
using Azos.Wave.Templatization;

namespace Tutorial.Web
{
  ///<summary>
  /// Displays branded internal login page, used for OAuth etc..
  ///</summary>
  public class TutorialLoginPage : WaveTemplate
  {
    public TutorialLoginPage(User clAppUser, string roundtrip, string error)
    {
      ClientUser = clAppUser;
      Roundtrip = roundtrip;
      Error = error;
    }


    public readonly User ClientUser;
    public readonly string Roundtrip;
    public readonly string Error;

    protected override void DoRender()
    {
      var error = RenderingContext as Exception;
      var content = GetType().GetText("Login.html");

      content = content.Replace("{{CLIENT_NAME}}", ClientUser.Description)
                       .Replace("{{CLIENT_ID}}", ClientUser.Name)
                       .Replace("{{ERROR}}", Error)
                       .Replace("{{ROUNDTRIP}}", Roundtrip)
                       .Replace("{{TIME}}", Ambient.UTCNow.ToString());

      Target.Write(content);
    }

  }//class
}
