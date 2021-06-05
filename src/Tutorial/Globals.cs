/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.IO;
using System.Reflection;

using Azos;

namespace Tutorial
{
  /// <summary>
  /// Contains global value accessors such as the ones for SKY_HOME, SKY_DEV_HOME etc.
  /// </summary>
  public static class Globals
  {
    /// <summary>
    /// The environment variable name which points to development source code home
    /// </summary>
    public const string ENV_VAR_DEV_HOME = "SKY_DEV_HOME";

    /// <summary>
    /// The name of the environment variable where programs put.take artifacts (such as log files) to/from
    /// </summary>
    public const string ENV_VAR_SKY_HOME = "SKY_HOME";


    /// <summary>
    /// The name of the environment variable which stores a `ushort` number used as a node discriminator within a local cluster of multiple nodes
    /// </summary>
    public const string ENV_VAR_NODE_DISCRIMINATOR = "SKY_NODE_DISCRIMINATOR";


    public static readonly Azos.Conf.BuildInformation THIS_BUILD_INFO = new Azos.Conf.BuildInformation(Assembly.GetExecutingAssembly());


    private static string s_DevHomePath;
    private static string s_HomePath;
    private static ushort? s_Discriminator;

    /// <summary>
    /// Provides a safe read accessor for ENV_VAR_DEV_HOME environment variable value
    /// </summary>
    public static string DevHomePath
    {
      get
      {
        var path = s_DevHomePath;
        if (path.IsNotNullOrWhiteSpace()) return path;

        path = Environment.GetEnvironmentVariable(ENV_VAR_DEV_HOME);
        if (path.IsNullOrWhiteSpace())
          throw new TutorialException(StringConsts.SKY_VAR_NOT_CONFIGURED_ERROR.Args(nameof(DevHomePath), ENV_VAR_DEV_HOME));

        if (!Directory.Exists(path))
          throw new TutorialException(StringConsts.SKY_VAR_BAD_PATH_ERROR.Args(nameof(DevHomePath), ENV_VAR_DEV_HOME, path));

        s_DevHomePath = path;

        return path;
      }
    }

    /// <summary>
    /// Provides a safe read accessor for SKY_HOME environment variable value
    /// </summary>
    public static string HomePath
    {
      get
      {
        var path = s_HomePath;
        if (path.IsNotNullOrWhiteSpace()) return path;

        path = Environment.GetEnvironmentVariable(ENV_VAR_SKY_HOME);
        if (path.IsNullOrWhiteSpace())
          throw new TutorialException(StringConsts.SKY_VAR_NOT_CONFIGURED_ERROR.Args(nameof(HomePath), ENV_VAR_SKY_HOME));

        if (!Directory.Exists(path))
          throw new TutorialException(StringConsts.SKY_VAR_BAD_PATH_ERROR.Args(nameof(HomePath), ENV_VAR_SKY_HOME, path));

        s_HomePath = path;

        return path;
      }
    }


    /// <summary>
    /// Provides a safe read accessor for SKY_NODE_DISCRIMINATOR environment variable value.
    /// A node discriminator value is a `ushort` number used within a local cluster of multiple nodes to
    /// distinguish individual nodes from each other
    /// </summary>
    public static ushort NodeDiscriminator
    {
      get
      {
        var got = s_Discriminator;
        if (got.HasValue) return got.Value;

        var ev = Environment.GetEnvironmentVariable(ENV_VAR_NODE_DISCRIMINATOR);

        if (!ushort.TryParse(ev, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var val))
          throw new TutorialException(StringConsts.SKY_VAR_NOT_CONFIGURED_ERROR.Args(nameof(NodeDiscriminator), ENV_VAR_NODE_DISCRIMINATOR));


        s_Discriminator = val;

        return val;
      }
    }

    /// <summary>
    /// Provides a safe read accessor for SKY_NODE_DISCRIMINATOR environment variable value - high order byte value.
    /// A node discriminator value is a `ushort` number used within a local cluster of multiple nodes to
    /// distinguish individual nodes from each other
    /// </summary>
    public static byte NodeDiscriminatorHi => (byte)(NodeDiscriminator >> 8);

    /// <summary>
    /// Provides a safe read accessor for SKY_NODE_DISCRIMINATOR environment variable value - low order byte value.
    /// A node discriminator value is a `ushort` number used within a local cluster of multiple nodes to
    /// distinguish individual nodes from each other
    /// </summary>
    public static byte NodeDiscriminatorLow => (byte)(NodeDiscriminator & 0xff);

  }
}
