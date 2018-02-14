using System;
using Spk.UnhandledExceptionHandlerCore.Configuration;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public static class ConfigUtils
    {
        private static ErrorHandlingSection ErrorHandlingSection { get; set; }

        public static string SentryDsn
        {
            get { return GetErrorHandlingSection().SentryDsn; }
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