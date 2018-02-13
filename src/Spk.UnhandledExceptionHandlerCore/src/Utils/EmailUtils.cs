using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class EmailUtils
    {
        private static readonly List<string> Crawlers = new List<string>
        {
            "googlebot",
            "google.com/bot.html",
            "bot",
            "crawler",
            "feed",
            "rss",
            "spider",
            "80legs",
            "baidu",
            "slurp", // was "yahoo! slurp" but didn't seem to work correctly
            "ia_archiver",
            "catalog",
            "mediapartners-google",
            "lwp-trivial",
            "nederland.zoek",
            "ahoy",
            "anthill",
            "appie",
            "arale",
            "araneo",
            "ariadne",
            "zapier",
            "atn_worldwide",
            "atomz",
            "bjaaland",
            "ukonline",
            "calif",
            "combine",
            "cosmos",
            "jakarta",
            "cusco",
            "cyberspyder",
            "digger",
            "grabber",
            "downloadexpress",
            "ecollector",
            "ebiness",
            "esculapio",
            "esther",
            "felix ide",
            "hamahakki",
            "kit-fireball",
            "fouineur",
            "freecrawl",
            "desertrealm",
            "gcreep",
            "golem",
            "griffon",
            "gromit",
            "gulliver",
            "gulper",
            "whowhere",
            "havindex",
            "hotwired",
            "htdig",
            "ingrid",
            "informant",
            "inspectorwww",
            "iron33",
            "teoma",
            "ask jeeves",
            "jeeves",
            "image.kapsi.net",
            "kdd-explorer",
            "label-grabber",
            "larbin",
            "linkidator",
            "linkwalker",
            "lockon",
            "marvin",
            "mattie",
            "mediafox",
            "merzscope",
            "nec-meshexplorer",
            "udmsearch",
            "moget",
            "motor",
            "muncher",
            "muninn",
            "muscatferret",
            "mwdsearch",
            "sharp-info-agent",
            "webmechanic",
            "netscoop",
            "newscan-online",
            "objectssearch",
            "orbsearch",
            "packrat",
            "pageboy",
            "parasite",
            "patric",
            "pegasus",
            "phpdig",
            "piltdownman",
            "pimptrain",
            "plumtreewebaccessor",
            "getterrobo-plus",
            "raven",
            "roadrunner",
            "robbie",
            "robocrawl",
            "robofox",
            "webbandit",
            "scooter",
            "search-au",
            "searchprocess",
            "senrigan",
            "shagseeker",
            "site valet",
            "skymob",
            "slurp",
            "snooper",
            "speedy",
            "curl_image_client",
            "suke",
            "www.sygol.com",
            "tach_bw",
            "templeton",
            "titin",
            "topiclink",
            "udmsearch",
            "urlck",
            "valkyrie libwww-perl",
            "verticrawl",
            "victoria",
            "webscout",
            "voyager",
            "crawlpaper",
            "webcatcher",
            "t-h-u-n-d-e-r-s-t-o-n-e",
            "webmoose",
            "pagesinventory",
            "webquest",
            "webreaper",
            "webwalker",
            "winona",
            "occam",
            "robi",
            "fdse",
            "jobo",
            "rhcs",
            "gazz",
            "dwcp",
            "yeti",
            "fido",
            "wlm",
            "wolp",
            "wwwc",
            "xget",
            "legs",
            "curl",
            "webs",
            "wget",
            "sift",
            "cmc",
            "updown_tester",
            "facebook"
        };

        private static readonly List<string> UnwantedUriParts = new List<string>
        {
            "remote.axd",
            "favicon",
            "apple-" // Apple icons
        };

        private static readonly List<string> UnwantedHttpExceptions = new List<string>
        {
            "the length of the url for this request exceeds",
            "a potentially dangerous request",
            "the remote host closed the connection"
        };

        private static readonly List<string> UnwantedArgumentExceptions = new List<string>
        {
            "illegal characters in path"
        };

        private static readonly List<string> UnwantedInvalidOperationExceptions = new List<string>
        {
            "the requested resource can only be accessed via ssl" // SSL website accessed by bot not handling SSL
        };

        public static bool ShouldSendEmail(Exception exception)
        {
            try
            {
                // Ignore when working on local environment
                if (!ConfigUtils.SendWhenLocal && HttpContext.Current.Request.Url.Host.Contains(".local"))
                    return false;

                // Ignore crawler requests
                if (ConfigUtils.IgnoreCrawlers && HttpContext.Current.Request.UserAgent != null)
                {
                    var agent = HttpContext.Current.Request.UserAgent.ToLowerInvariant();
                    if (Crawlers.Exists(agent.Contains))
                        return false;
                }

                // Ignore errors for some URI parts
                var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant();
                if (UnwantedUriParts.Exists(absoluteUri.Contains))
                    return false;

                // Ignore specified paths
                if (ConfigUtils.PathsToIgnore.Any(x =>
                    HttpContext.Current.Request.Url.ToString().ToLower().Contains(x)))
                    return false;
            }
            catch (HttpException)
            {
                // Request isn't available
            }

            // Ignore 404 errors
            if (exception is HttpException && ((HttpException)exception).GetHttpCode() == 404)
                return false;

            // Ignore some http exceptions
            if (exception is HttpException)
            {
                var message = exception.Message.ToLowerInvariant();
                if (UnwantedHttpExceptions.Exists(message.Contains))
                    return false;
            }

            // Ignore some argument exceptions
            if (exception is ArgumentException)
            {
                var message = exception.Message.ToLowerInvariant();
                if (UnwantedArgumentExceptions.Exists(message.Contains))
                    return false;
            }

            // Ignore some invalid operations exceptions
            if (exception is InvalidOperationException)
            {
                var message = exception.Message.ToLowerInvariant();
                if (UnwantedInvalidOperationExceptions.Exists(message.Contains))
                    return false;
            }

            if (exception is PathTooLongException)
                return false;

            if (exception is HttpAntiForgeryException)
                return false;

            return true;
        }

        public static void SendEmail(Exception exception)
        {
            var mail = new MailMessage();
            mail.To.Add(ConfigUtils.To);

            var fromAddress = ConfigUtils.From;

            // Add subject prefix to the "from" address
            if (!string.IsNullOrWhiteSpace(ConfigUtils.SubjectPrefix))
            {
                var pieces = fromAddress.Split('@');
                var glue = pieces[0].Contains("+") ? "_" : "+";

                if (pieces.Length == 2)
                    fromAddress = pieces[0] + glue + Slugify(ConfigUtils.SubjectPrefix) + "@" + pieces[1];
            }

            mail.From = new MailAddress(fromAddress, ConfigUtils.FromName);
            mail.Subject = "[" + ConfigUtils.SubjectPrefix + "] An unhandled error has been handled!";

            // MVC information
            mail.Body += "<h2>MVC Information</h2>";
            mail.Body += "<p>";

            // Request information
            mail.Body += "<h2>Request Information</h2>";

            try
            {
                var request = HttpContext.Current.Request;

                mail.Body += "<p>";
                mail.Body += "HttpMethod: " + request.HttpMethod + "<br/>";
                mail.Body += "RawUrl: " + request.RawUrl + "<br/>";

                if (!string.IsNullOrEmpty(request.Url.AbsoluteUri))
                {
                    mail.Body += "AbsoluteUri: " + request.Url.AbsoluteUri + "<br/>";
                }

                if (request.UrlReferrer != null)
                {
                    mail.Body += "UrlReferrer: " + request.UrlReferrer + "<br/>";
                }

                if (request.UserAgent != null)
                {
                    mail.Body += "UserAgent: " + request.UserAgent + "<br/>";
                }

                mail.Body += "UserHostAddress: " + request.UserHostAddress + "<br/>";

                mail.Body += "</p>";

                // Form information
                var form = request.Form;
                mail.Body += "<h2>Form Information</h2>";

                mail.Body += "<p>";

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
                                ? "<i>[hidden]</i>"
                                : "<i>[would be hidden but empty]</i>";
                        }
                        else
                        {
                            value = Convert.ToString(form[key]);
                        }

                        mail.Body += key + ": " + value + "<br/>";
                    }
                }
                else
                {
                    mail.Body += "<p style='font-style: italic;'>Form is empty</p>";
                }

                mail.Body += "</p>";

                // Session information
                var session = HttpContext.Current.Session;
                mail.Body += "<h2>Session Information</h2>";

                if (session != null)
                {
                    mail.Body += "<p>";

                    if (session.Keys.Count > 0)
                    {
                        foreach (string key in session.Keys)
                        {
                            var value = session[key];

                            if (value is IList)
                            {
                                mail.Body += key + " (list): ";
                                mail.Body += "<p style='margin-top: 6px; padding-left: 20px;'>";

                                foreach (var enumValue in value as IList)
                                {
                                    mail.Body += Convert.ToString(enumValue) + "<br/>";
                                }

                                mail.Body += "</p>";
                            }
                            else
                            {
                                mail.Body += key + ": " + Convert.ToString(value) + "<br/>";
                            }
                        }
                    }
                    else
                    {
                        mail.Body += "<p style='font-style: italic;'>Session is empty</p>";
                    }

                    mail.Body += "</p>";
                }
                else
                {
                    mail.Body += "<p style='font-style: italic;'>No session was found</p>";
                }
            }
            catch (HttpException)
            {
                // Request isn't available
                mail.Body += "<p style='font-style: italic;'>Request isn't available in this context</p>";
            }

            // Main exception
            mail.Body += "<h2>Exception</h2>" +
                         "<h3>Type</h3>" + exception.GetType() +
                         "<h3>Message</h3>" + exception.Message +
                         "<h3>Stack Trace</h3><pre>" + exception.StackTrace + "</pre>";

            // All inner exceptions
            exception = exception.InnerException;
            while (exception != null)
            {
                mail.Body += "<h2>Inner Exception</h2>" +
                             "<h3>Type</h3>" + exception.GetType() +
                             "<h3>Message</h3>" + exception.Message +
                             "<h3>Stack Trace</h3><pre>" + exception.StackTrace + "</pre>";

                exception = exception.InnerException;
            }

            // Append custom message
            if (ErrorHandlingUtils.MessageCustomizer != null)
                mail.Body += ErrorHandlingUtils.MessageCustomizer.AppendToErrorMessage();

            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;

            var smtp = ConfigUtils.UseCustomSmtpConfig
                ? new SmtpClient
                {
                    Host = ConfigUtils.SmtpHost,
                    Port = ConfigUtils.SmtpPort,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ConfigUtils.Username, ConfigUtils.Password),
                    EnableSsl = ConfigUtils.EnableSsl
                }
                : new SmtpClient();

            smtp.Send(mail);
        }

        /// <summary>
        ///     Copied from https://coderwall.com/p/vshjwq/how-to-generate-clean-url-slug-in-c
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Slugify(string value)
        {
            //First to lower case 
            value = value.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);

            value = Encoding.ASCII.GetString(bytes);

            //Replace spaces 
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars 
            value = Regex.Replace(value, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);

            //Trim dashes from end 
            value = value.Trim('-', '_');

            //Replace double occurences of - or \_ 
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }
    }
}