/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/


using System;

using Azos;
using Azos.Security;
using Azos.Security.Services;
using Azos.Security.Tokens;
using Azos.Serialization.JSON;

namespace Tutorial.Web.Controllers
{
  /// <summary>
  /// Injects custom login page and extra JWT claims for OAuth flows
  /// NOTE: This is an OAuth/OIDC SERVER, not client
  /// </summary>
  public abstract class TutorialOAuthController : OAuthControllerBase
  {
    public TutorialOAuthController() : base() { }
    protected TutorialOAuthController(IOAuthModule oauth) : base(oauth) { }


    protected override object MakeAuthorizeResult(User clientUser, string roundtrip, string error)
    {
      if (WorkContext.RequestedJson)
      {
        return new { OK = error.IsNullOrEmpty(), roundtrip, error };
      }

      return new TutorialLoginPage(clientUser, roundtrip, error);
    }

    //This is an example how to add additional JWT token fields with OAuth server
    protected override void AddExtraClaimsToIDToken(User clientUser, User subjectUser, AccessToken accessToken, JsonDataMap jwtClaims)
    {
      //https://openid.net/specs/openid-connect-core-1_0.html#TokenResponse
      //https://www.iana.org/assignments/jwt/jwt.xhtml#claims
      //https://auth0.com/docs/tokens/jwt-claims
      //https://developer.okta.com/blog/2017/07/25/oidc-primer-part-1
      //https://developer.okta.com/blog/2017/07/25/oidc-primer-part-2
      //https://developer.okta.com/blog/2017/08/01/oidc-primer-part-3

      base.AddExtraClaimsToIDToken(clientUser, subjectUser, accessToken, jwtClaims);

      //if (subjectUser is TutorialUser tuser &&  tuser.RealmUserData is Security.MyUserData data)
      //{
      //  jwtClaims["email"] = data.EMail;
      //  jwtClaims["locale"] = "en-US";
      //}
    }

  }
}
