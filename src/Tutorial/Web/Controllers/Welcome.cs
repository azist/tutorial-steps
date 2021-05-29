/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/


using System;

using Azos.Serialization.JSON;
using Azos.Wave.Mvc;

namespace Tutorial.Web.Controllers
{
  public class Welcome : Controller
  {
    [Action]
    public object Index()
    {
      return "Welcome to API server";
    }

    [Action]
    public object Echo(JsonDataMap got)
    {
      return new {got, now = App.TimeSource.UTCNow};
    }
  }
}
