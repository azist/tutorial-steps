/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Text;

using Azos.Security;

namespace Tutorial.Security.Permissions
{
  /// <summary>
  /// Marker interface denoting permissions in this system.
  /// </summary>
  public interface ITutorialPermission { }


  /// <summary>
  /// Provides base for basic permissions used by Tutorial systems.
  /// </summary>
  public abstract class TutorialPermission : TypedPermission, ITutorialPermission
  {
    /// <summary>
    /// Creates the check instance against the minimum access level for this typed permission
    /// </summary>
    protected TutorialPermission(int level) : base(level)
    {
    }
  }

  /// <summary>
  /// Provides abstraction for permissions that operate in a context of a data store,
  /// so their ACL/rights vectors contain sub-sections. The data store context is supplied
  /// via session.DataContextName which is comprised of segments delimited by commas/spaces,
  /// each segment string addressing target data context's by name. For example: "client1,
  /// devmaster" The root permission level definition must be granted first, then system
  /// tries to find specifically named context each prefixed with `data::`. If it can
  /// not be found, the system defaults to the node `data::*` if it is found, otherwise
  /// access is denied.
  /// </summary>
  public abstract class TutorialDataContextualPermission : DataContextualPermission, ITutorialPermission
  {
    /// <summary>
    /// Creates the check instance against the minimum access level for this typed permission
    /// </summary>
    protected TutorialDataContextualPermission(int level) : base(level)
    {
    }
  }
}
