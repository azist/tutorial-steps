/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

namespace Tutorial
{
  /// <summary>
  /// Localizable system-wide constants
  /// </summary>
  public static class StringConsts
  {
    public const string ARGUMENT_ERROR = "Argument error: ";

    public const string SKY_VAR_NOT_CONFIGURED_ERROR = "Could not read {0} - the machine environment variable `{1}` is not configured";
    public const string SKY_VAR_BAD_PATH_ERROR = "Could not read {0} - the machine environment variable `{1}` contains path `{2}` which does not exist";
  }
}
