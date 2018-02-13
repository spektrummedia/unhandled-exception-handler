using System;
using Spk.UnhandledExceptionHandlerCore.Configuration;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public static class ConfigUtils
    {
        private static ErrorHandlingSection ErrorHandlingSection { get; set; }

        public static bool UseCustomSmtpConfig
        {
            get { return GetErrorHandlingSection().UseCustomSmtpConfig; }
        }

        public static string SmtpHost
        {
            get { return GetErrorHandlingSection().SmtpHost; }
        }

        public static int SmtpPort
        {
            get { return GetErrorHandlingSection().SmtpPort; }
        }

        public static bool EnableSsl
        {
            get { return GetErrorHandlingSection().EnableSsl; }
        }

        public static string Username
        {
            get { return GetErrorHandlingSection().EmailUsername; }
        }

        public static string Password
        {
            get { return GetErrorHandlingSection().EmailPassword; }
        }

        public static string From
        {
            get { return GetErrorHandlingSection().EmailFrom; }
        }

        public static string FromName
        {
            get { return GetErrorHandlingSection().EmailFromName; }
        }

        public static string To
        {
            get { return GetErrorHandlingSection().EmailTo; }
        }

        public static string SubjectPrefix
        {
            get { return GetErrorHandlingSection().EmailSubjectPrefix; }
        }

        public static bool SendWhenLocal
        {
            get { return GetErrorHandlingSection().SendEmailWhenLocal; }
        }

        public static bool IgnoreCrawlers
        {
            get { return GetErrorHandlingSection().IgnoreCrawlers; }
        }

        public static string BaseControllerPath
        {
            get { return GetErrorHandlingSection().BaseControllerPath; }
        }

        public static bool ShowErrorsWhenLocal
        {
            get { return GetErrorHandlingSection().ShowErrorsWhenLocal; }
        }

        public static string[] PathsToIgnore
        {
            get
            {
                var pathsToIgnore = GetErrorHandlingSection().PathsToIgnore;

                if (String.IsNullOrWhiteSpace(pathsToIgnore))
                    return new string[0];

                string[] delimiters = { "," };

                return pathsToIgnore.Replace(" ", "")
                    .ToLowerInvariant()
                    .Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            }
        }


        public static string[] FieldsToHide
        {
            get
            {
                var fieldsToHide = GetErrorHandlingSection().FieldsToHide;

                if (String.IsNullOrWhiteSpace(fieldsToHide))
                    return new string[0];

                string[] delimiters = { "," };

                return fieldsToHide.Replace(" ", "")
                    .ToLowerInvariant()
                    .Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private static ErrorHandlingSection GetErrorHandlingSection()
        {
            // Load only if null
            if (ErrorHandlingSection == null)
                ErrorHandlingSection =
                    (ErrorHandlingSection)
                        System.Configuration.ConfigurationManager.GetSection("errorHandlingGroup/errorHandling");

            // If still null, instanciate with default values
            return ErrorHandlingSection ?? (ErrorHandlingSection = new ErrorHandlingSection());
        }
    }
}