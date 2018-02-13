using System.Web.Mvc;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public static class ErrorHandlingUtils
    {
        public static MessageCustomizer MessageCustomizer { get; set; }

        public static ErrorHandle ErrorHandle { get; set; }
    }

    /// <summary>
    /// This class has been created to help customizing the error message that is sent by email by this package. This
    /// is achieved by extending the class, overriding its methods, and assigning it to MessageCustomizer in
    /// ErrorHandlingUtils.
    /// </summary>
    public class MessageCustomizer
    {
        /// <summary>
        /// Returns an empty string by default. This can be easily overriden to append more information to error
        /// messages sent by email.
        /// </summary>
        /// <returns>A string that will be appended to the error message.</returns>
        public virtual string AppendToErrorMessage()
        {
            return "";
        }
    }

    /// <summary>
    /// This class has been created to help custom handling of specific error/exception types. This is achieved by
    /// extending the class, overriding its methods, and assigning it to ErrorHandle in ErrorHandlingUtils.
    /// </summary>
    public class ErrorHandle
    {
        /// <summary>
        /// Does nothing by default. This can be easily overriden to add custom handling of anti forgery token.
        /// </summary>
        /// <param name="exception">The exception that was thrown by the application, and caught by the error
        /// handler.</param>
        public virtual void OnAntiForgeryException(HttpAntiForgeryException exception)
        {

        }
    }
}
