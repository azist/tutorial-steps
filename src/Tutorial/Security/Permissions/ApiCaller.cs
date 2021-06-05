/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using Azos.Security;

namespace Tutorial.Security.Permissions
{

  /// <summary>
  /// Denotes the scopes of APIs that the caller can access.
  /// Based on AccessLevels 0..3
  /// </summary>
  public enum ApiAccessScope
  {
    Denied = AccessLevel.DENIED,

    /// <summary>
    /// Caller can make calls to APis of this application as a typical user
    /// </summary>
    User = AccessLevel.VIEW,

    /// <summary>
    /// Caller gets additional privileges, typically used for services for inter-node communication
    /// </summary>
    Service = AccessLevel.VIEW_CHANGE
  }


  /// <summary>
  /// Stipulates that the bearer must be able to make any kind of API calls within this system
  /// </summary>
  public sealed class ApiCaller : TutorialPermission
  {
    /// <summary>
    /// Use this property for imperative checks in code: if (ApiCaller.Instance.Check(app, user)) ...
    /// </summary>
    public static readonly ApiCaller Instance = new ApiCaller();

    public ApiCaller() : base(AccessLevel.VIEW) { }
    public ApiCaller(ApiAccessScope scope) : base((int)scope) { }

    public override string Description => "Can call Apis";
  }
}

