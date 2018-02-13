using System;
using System.Security;
using System.Web;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class AppErrorUtils
    {
        public static void OnApplicationError(object sender, EventArgs e, Exception exception)
        {
            if (exception == null)
                return;

            var httpContext = ((HttpApplication)sender).Context;

            // Log the error
            LoggingUtils.LogAllExceptions(exception);

            // Send error by email if it has to be done
            if (EmailUtils.ShouldSendEmail(exception))
                EmailUtils.SendEmail(exception);

            try
            {
                if (ConfigUtils.ShowErrorsWhenLocal && httpContext.Request.Url.Host.Contains(".local"))
                    return;
            }
            catch (HttpException)
            {
                // Request isn't available
            }

            var action = "AppError";

            if (exception is HttpException)
            {
                var httpEx = exception as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;
                    case 403:
                        action = "AccessDenied";
                        break;
                    case 401:
                        action = "AccessDenied";
                        break;
                }
            }

            if (exception is SecurityException)
            {
                action = "AccessDenied";
            }

            try
            {
                httpContext.ClearError();
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = exception is HttpException
                    ? ((HttpException)exception).GetHttpCode()
                    : 500;
                httpContext.Response.TrySkipIisCustomErrors = true;
                httpContext.Response.TransmitFile($"~/Views/Error/{action}.html");
            }
            catch (HttpException)
            {
                // Response isn't available
            }
        }
    }
}