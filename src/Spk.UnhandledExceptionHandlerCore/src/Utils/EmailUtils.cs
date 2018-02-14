using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SharpRaven;
using SharpRaven.Data;

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
                {
                    return false;
                }

                // Ignore crawler requests
                if (ConfigUtils.IgnoreCrawlers && HttpContext.Current.Request.UserAgent != null)
                {
                    var agent = HttpContext.Current.Request.UserAgent.ToLowerInvariant();
                    if (Crawlers.Exists(agent.Contains))
                    {
                        return false;
                    }
                }

                // Ignore errors for some URI parts
                var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant();
                if (UnwantedUriParts.Exists(absoluteUri.Contains))
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
                if (UnwantedHttpExceptions.Exists(message.Contains))
                {
                    return false;
                }
            }

            // Ignore some argument exceptions
            if (exception is ArgumentException)
            {
                var message = exception.Message.ToLowerInvariant();
                if (UnwantedArgumentExceptions.Exists(message.Contains))
                {
                    return false;
                }
            }

            // Ignore some invalid operations exceptions
            if (exception is InvalidOperationException)
            {
                var message = exception.Message.ToLowerInvariant();
                if (UnwantedInvalidOperationExceptions.Exists(message.Contains))
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