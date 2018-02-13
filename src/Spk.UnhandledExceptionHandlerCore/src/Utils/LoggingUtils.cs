using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using NLog;
using Spk.UnhandledExceptionHandlerCore.Logging;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class LoggingUtils
    {
        private const string FatSeparator =
            "========================================================================================================";
        private const string SlimSeparator =
            "--------------------------------------------------------------------------------------------------------";
        private const string TitleSeparator =
            "-------------------------------------------------";

        public static Logger LoggerInstance()
        {
            return TheLogger.Instance;
        }

        public static void LogAllExceptions(Exception exception)
        {
            // Build error header message
            var sb = new StringBuilder();

            // Add MVC information
            sb.AppendLine();
            sb.AppendLine(FatSeparator);
            sb.AppendLine("An unhandled error has been handled!");
            sb.AppendLine(SlimSeparator);
            sb.AppendLine();

            // Add request information
            sb.AppendLine(TitleSeparator);
            sb.AppendLine("Request Information");
            sb.AppendLine(TitleSeparator);
            sb.AppendLine();

            try
            {
                var request = HttpContext.Current.Request;

                sb.AppendLine("HttpMethod: " + request.HttpMethod);
                sb.AppendLine("RawUrl: " + request.RawUrl);

                if (!string.IsNullOrEmpty(request.Url.AbsoluteUri))
                    sb.AppendLine("AbsoluteUri: " + request.Url.AbsoluteUri);

                if (request.UrlReferrer != null)
                    sb.AppendLine("UrlReferrer: " + request.UrlReferrer);

                if (request.UserAgent != null)
                    sb.AppendLine("UserAgent: " + request.UserAgent);

                sb.AppendLine("UserHostAddress: " + request.UserHostAddress);
                sb.AppendLine();

                // Add form information
                sb.AppendLine(TitleSeparator);
                sb.AppendLine("Form Information");
                sb.AppendLine(TitleSeparator);
                sb.AppendLine();

                var form = request.Form;

                if (form.Keys.Count > 0)
                {
                    var fieldsToHide = ConfigUtils.FieldsToHide;

                    foreach (string key in form.Keys)
                    {
                        string value;

                        // Prevent passwords to be shown
                        if (fieldsToHide.Contains(key.ToLowerInvariant()))
                        {
                            value = Convert.ToString(form[key]).Length > 0
                                ? "[hidden]"
                                : "[would be hidden but empty]";
                        }
                        else
                        {
                            value = Convert.ToString(form[key]);
                        }

                        sb.AppendLine(key + ": " + value);
                    }
                }
                else
                {
                    sb.AppendLine("*** Form is empty ***");
                }

                sb.AppendLine();

                // Add Session information
                sb.AppendLine(TitleSeparator);
                sb.AppendLine("Session Information");
                sb.AppendLine(TitleSeparator);
                sb.AppendLine();

                var session = HttpContext.Current.Session;

                if (session != null)
                {
                    if (session.Keys.Count > 0)
                    {
                        foreach (string key in session.Keys)
                        {
                            var value = session[key];

                            if (value is IList)
                            {
                                sb.AppendLine(key + " (list):");

                                foreach (var enumValue in value as IList)
                                {
                                    sb.AppendLine("  > " + Convert.ToString(enumValue));
                                }
                            }
                            else
                            {
                                sb.AppendLine(key + ": " + Convert.ToString(value));
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine("*** Session is empty ***");
                    }
                }
                else
                {
                    sb.AppendLine("*** No session was found ***");
                }

                sb.AppendLine();
            }
            catch (HttpException)
            {
                // Request isn't available
                sb.AppendLine("*** Request is not available in this context ***");
                sb.AppendLine();
            }

            // Main exception
            sb.AppendLine(SlimSeparator);
            sb.AppendLine("Exception");
            sb.AppendLine(SlimSeparator);
            sb.AppendLine();

            sb.AppendLine("Type");
            sb.AppendLine("====");
            sb.AppendLine(exception.GetType().ToString());
            sb.AppendLine();

            sb.AppendLine("Message");
            sb.AppendLine("=======");
            sb.AppendLine(exception.Message);
            sb.AppendLine();

            sb.AppendLine("Stack Trace");
            sb.AppendLine("===========");
            sb.AppendLine(exception.StackTrace);
            sb.AppendLine();

            // All inner exceptions
            exception = exception.InnerException;
            while (exception != null)
            {
                sb.AppendLine(SlimSeparator);
                sb.AppendLine("Inner Exception");
                sb.AppendLine(SlimSeparator);
                sb.AppendLine();

                sb.AppendLine("Type");
                sb.AppendLine("====");
                sb.AppendLine(exception.GetType().ToString());
                sb.AppendLine();

                sb.AppendLine("Message");
                sb.AppendLine("=======");
                sb.AppendLine(exception.Message);
                sb.AppendLine();

                sb.AppendLine("Stack Trace");
                sb.AppendLine("===========");
                sb.AppendLine(exception.StackTrace);
                sb.AppendLine();

                exception = exception.InnerException;
            }

            LoggerInstance().Fatal(sb.ToString());
        }
    }
}