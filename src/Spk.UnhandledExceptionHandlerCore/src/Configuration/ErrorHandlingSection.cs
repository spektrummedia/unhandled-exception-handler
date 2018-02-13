﻿using System.Configuration;

namespace Spk.UnhandledExceptionHandlerCore.Configuration
{
    public class ErrorHandlingSection : ConfigurationSection
    {
        [ConfigurationProperty("useCustomSmtpConfig", DefaultValue = "false", IsRequired = true)]
        public bool UseCustomSmtpConfig
        {
            get
            {
                return (bool)this["useCustomSmtpConfig"];
            }
            set
            {
                this["useCustomSmtpConfig"] = value;
            }
        }

        [ConfigurationProperty("smtpHost", DefaultValue = "", IsRequired = true)]
        public string SmtpHost
        {
            get
            {
                return (string)this["smtpHost"];
            }
            set
            {
                this["smtpHost"] = value;
            }
        }

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

        [ConfigurationProperty("smtpPort", DefaultValue = "587", IsRequired = true)]
        public int SmtpPort
        {
            get
            {
                return (int)this["smtpPort"];
            }
            set
            {
                this["smtpPort"] = value;
            }
        }

        [ConfigurationProperty("enableSsl", DefaultValue = "true", IsRequired = true)]
        public bool EnableSsl
        {
            get
            {
                return (bool)this["enableSsl"];
            }
            set
            {
                this["enableSsl"] = value;
            }
        }

        [ConfigurationProperty("emailUsername", DefaultValue = "", IsRequired = true)]
        public string EmailUsername
        {
            get
            {
                return (string)this["emailUsername"];
            }
            set
            {
                this["emailUsername"] = value;
            }
        }

        [ConfigurationProperty("emailPassword", DefaultValue = "", IsRequired = true)]
        public string EmailPassword
        {
            get
            {
                return (string)this["emailPassword"];
            }
            set
            {
                this["emailPassword"] = value;
            }
        }

        [ConfigurationProperty("emailFrom", DefaultValue = "", IsRequired = true)]
        public string EmailFrom
        {
            get
            {
                return (string)this["emailFrom"];
            }
            set
            {
                this["emailFrom"] = value;
            }
        }

        [ConfigurationProperty("emailFromName", DefaultValue = "", IsRequired = true)]
        public string EmailFromName
        {
            get
            {
                return (string)this["emailFromName"];
            }
            set
            {
                this["emailFromName"] = value;
            }
        }

        [ConfigurationProperty("emailTo", DefaultValue = "", IsRequired = true)]
        public string EmailTo
        {
            get
            {
                return (string)this["emailTo"];
            }
            set
            {
                this["emailTo"] = value;
            }
        }

        [ConfigurationProperty("emailSubjectPrefix", DefaultValue = "Unknown Project", IsRequired = true)]
        public string EmailSubjectPrefix
        {
            get
            {
                return (string)this["emailSubjectPrefix"];
            }
            set
            {
                this["emailSubjectPrefix"] = value;
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