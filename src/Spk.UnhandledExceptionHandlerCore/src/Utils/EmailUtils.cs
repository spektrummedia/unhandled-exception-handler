using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharpRaven;
using SharpRaven.Data;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class EmailUtils
    {
        public static bool ShouldSendEmail(Exception exception)
        {
            try
            {
                // Ignore when working on local environment
                if (!ConfigUtils.SendWhenLocal && HttpContext.Current.Request.Url.Host.Contains(".local"))
                {
                    return false;
                }

                // Ignore crawler requests
                if (ConfigUtils.IgnoreCrawlers && HttpContext.Current.Request.UserAgent != null)
                {
                    var agent = HttpContext.Current.Request.UserAgent.ToLowerInvariant();
                    if (Constants.Crawlers.Exists(agent.Contains))
                    {
                        return false;
                    }
                }

                // Ignore errors for some URI parts
                var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant();
                if (Constants.UnwantedUriParts.Exists(absoluteUri.Contains))
                {
                    return false;
                }

                // Ignore specified paths
                if (ConfigUtils.PathsToIgnore.Any(x =>
                    HttpContext.Current.Request.Url.ToString().ToLower().Contains(x)))
                {
                    return false;
                }
            }
            catch (HttpException)
            {
                // Request isn't available
            }

            // Ignore 404 errors
            if (exception is HttpException && ((HttpException) exception).GetHttpCode() == 404)
            {
                return false;
            }

            // Ignore some http exceptions
            if (exception is HttpException)
            {
                var message = exception.Message.ToLowerInvariant();
                if (Constants.UnwantedHttpExceptions.Exists(message.Contains))
                {
                    return false;
                }
            }

            // Ignore some argument exceptions
            if (exception is ArgumentException)
            {
                var message = exception.Message.ToLowerInvariant();
                if (Constants.UnwantedArgumentExceptions.Exists(message.Contains))
                {
                    return false;
                }
            }

            // Ignore some invalid operations exceptions
            if (exception is InvalidOperationException)
            {
                var message = exception.Message.ToLowerInvariant();
                if (Constants.UnwantedInvalidOperationExceptions.Exists(message.Contains))
                {
                    return false;
                }
            }

            if (exception is PathTooLongException)
            {
                return false;
            }

            if (exception is HttpAntiForgeryException)
            {
                return false;
            }

            return true;
        }

        public static void SendEmail(Exception exception)
        {
            var sentryClient = new RavenClient(ConfigUtils.SentryDsn);
            var request = HttpContext.Current.Request == null
                ? null
                : new HttpRequestWrapper(HttpContext.Current.Request);
            var session = HttpContext.Current.Session;

            try
            {
                var builder = new ExceptionWithDataBuilder(exception, request, session);
                sentryClient.Capture(new SentryEvent(builder.Build()));
            }
            catch
            {
                // So weird. We need to log it
                sentryClient.Capture(new SentryEvent(exception));
            }
        }
    }
}