/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using Azos.Security.Services;
using Azos.Wave.Mvc;

using Tutorial.Web.Controllers;

namespace Tutorial.Server.Web.SysControllers
{
  [NoCache]
  [ApiControllerDoc(
    BaseUri ="/system/oauth",
    ResponseHeaders = new[]{ ApiProtocolController.API_DOC_HDR_NO_CACHE }
  )]
  public sealed class OAuth : TutorialOAuthController
  {
    public OAuth(): base(){ }
    public OAuth(IOAuthModule oauth) : base(oauth) { }
  }
}
