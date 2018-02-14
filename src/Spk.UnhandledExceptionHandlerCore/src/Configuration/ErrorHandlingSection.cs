using System.Configuration;

namespace Spk.UnhandledExceptionHandlerCore.Configuration
{
    public class ErrorHandlingSection : ConfigurationSection
    {
        [ConfigurationProperty("pathsToIgnore", DefaultValue = "wp-content", IsRequired = true)]
        public string PathsToIgnore
        {
            get
            {
                return (string)this["pathsToIgnore"];
            }
            set
            {
                this["pathsToIgnore"] = value;
            }
        }

        [ConfigurationProperty("sentryDsn", DefaultValue = "", IsRequired = true)]
        public string SentryDsn
        {
            get
            {
                return (string)this["sentryDsn"];
            }
            set
            {
                this["sentryDsn"] = value;
            }
        }

        
        [ConfigurationProperty("sendEmailWhenLocal", DefaultValue = "false", IsRequired = true)]
        public bool SendEmailWhenLocal
        {
            get
            {
                return (bool)this["sendEmailWhenLocal"];
            }
            set
            {
                this["sendEmailWhenLocal"] = value;
            }
        }

        [ConfigurationProperty("ignoreCrawlers", DefaultValue = "true", IsRequired = true)]
        public bool IgnoreCrawlers
        {
            get
            {
                return (bool)this["ignoreCrawlers"];
            }
            set
            {
                this["ignoreCrawlers"] = value;
            }
        }

        [ConfigurationProperty("baseControllerPath", DefaultValue = "/app", IsRequired = true)]
        public string BaseControllerPath
        {
            get
            {
                return (string)this["baseControllerPath"];
            }
            set
            {
                this["baseControllerPath"] = value;
            }
        }

        [ConfigurationProperty("fieldsToHide", DefaultValue = "password", IsRequired = true)]
        public string FieldsToHide
        {
            get
            {
                return (string)this["fieldsToHide"];
            }
            set
            {
                this["fieldsToHide"] = value;
            }
        }

        [ConfigurationProperty("showErrorsWhenLocal", DefaultValue = "true", IsRequired = true)]
        public bool ShowErrorsWhenLocal
        {
            get
            {
                return (bool)this["showErrorsWhenLocal"];
            }
            set
            {
                this["showErrorsWhenLocal"] = value;
            }
        }
    }
}
