/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

using Azos;
using Azos.Conf;

namespace s01_console
{
  /// <summary>
  /// Provides base abstraction for unit of work
  /// </summary>
  public abstract class Work : IConfigurable
  {
    public const string CONFIG_WORK_SECTION = "work";

    public virtual void Configure(IConfigSectionNode node)
      => ConfigAttribute.Apply(this, node);//configure my properties

    /// <summary>
    /// Performs the work, override to do what you want, returning exit code
    /// </summary>
    public abstract int Perform();
  }

  /// <summary>
  /// This class just prints message
  /// </summary>
  /// <example>
  /// $ dotnet s01-console.dll
  /// $ dotnet s01-console.dll -silent
  /// $ dotnet s01-console.dll -work message="My special message"
  /// </example>
  public sealed class DefaultWork : Work
  {
    [Config(Default = "Default message text")]
    public string Message{ get; set;}

    public override int Perform()
    {
      Console.WriteLine($"    Message is: {Message}");
      return 0;
    }
  }

  /// <summary>
  /// This class loops through numbers
  /// </summary>
  /// <example>
  /// # notice: you could inject any type using FQN form another assembly
  /// $ dotnet s01-console.dll -work type="s01_console.LoopWork, s01-console" from=7 to=9
  /// $ dotnet s01-console.dll -silent -work type="s01_console.LoopWork, s01-console" from=7 to=9
  /// $ dotnet s01-console.dll -silent -work type="s01_console.LoopWork, s01-console" from=5
  /// $ dotnet s01-console.dll -silent -work type="s01_console.LoopWork, s01-console" to=23
  /// </example>
  public sealed class LoopWork : Work
  {
    [Config(Default = 1)]
    public int From { get; set; }

    [Config(Default = 10)]
    public int To { get; set; }

    public override int Perform()
    {
      for(var i=From; i<=To; i++)
      {
        Console.WriteLine($"    Number is: {i}");
      }

      return 0;
    }
  }

}
