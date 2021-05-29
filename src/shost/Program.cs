/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

namespace shost
{
  public class Program
  {
    public static int Main(string[] args)
    {
      new Azos.Platform.Abstraction.NetCore.NetCore20Runtime();

      var entryPointDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
      Environment.CurrentDirectory = entryPointDir;

      return Azos.Apps.ApplicationHostProgramBody.ConsoleMain(args);
    }
  }
}
