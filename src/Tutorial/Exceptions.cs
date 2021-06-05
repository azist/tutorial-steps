/*<FILE_LICENSE>
 * Azos Tutorial
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Runtime.Serialization;

namespace Tutorial
{
  /// <summary>
  /// Marker interface for error conditions related to Tutorial logic
  /// </summary>
  public interface ITutorialError { }


  /// <summary>
  /// Base exception thrown by the code in this Tutorial assembly
  /// </summary>
  [Serializable]
  public class TutorialException : Exception, ITutorialError
  {
    public TutorialException() { }
    public TutorialException(string message) : base(message) { }
    public TutorialException(string message, Exception inner) : base(message, inner) { }
    protected TutorialException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }


}
