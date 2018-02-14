using System.Collections.Generic;

namespace Spk.UnhandledExceptionHandlerCore
{
    internal partial class Constants
    {
        public static readonly List<string> UnwantedHttpExceptions = new List<string>
        {
            "the length of the url for this request exceeds",
            "a potentially dangerous request",
            "the remote host closed the connection"
        };
    }
}