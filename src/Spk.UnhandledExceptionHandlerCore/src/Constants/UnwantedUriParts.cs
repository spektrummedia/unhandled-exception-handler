using System.Collections.Generic;

namespace Spk.UnhandledExceptionHandlerCore
{
    internal partial class Constants
    {
        public static readonly List<string> UnwantedUriParts = new List<string>
        {
            "remote.axd",
            "favicon",
            "apple-" // Apple icons
        };
    }
}