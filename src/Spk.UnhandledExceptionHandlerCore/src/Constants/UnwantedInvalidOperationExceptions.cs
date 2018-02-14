using System.Collections.Generic;

namespace Spk.UnhandledExceptionHandlerCore
{
    internal partial class Constants
    {
        public static readonly List<string> UnwantedInvalidOperationExceptions = new List<string>
        {
            "the requested resource can only be accessed via ssl" // SSL website accessed by bot not handling SSL
        };
    }
}