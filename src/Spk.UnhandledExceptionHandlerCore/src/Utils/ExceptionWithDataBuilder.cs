using System;
using System.Linq;
using System.Web;
using Spk.Common.Helpers.Guard;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilder
    {
        private readonly Exception _currentException;
        private readonly HttpRequestWrapper _request;

        public ExceptionWithDataBuilder(
            Exception exception,
            HttpRequestWrapper request)
        {
            exception.GuardIsNotNull(nameof(exception));
            request.GuardIsNotNull(nameof(request));

            _request = request;
            _currentException = exception;
        }

        public Exception Build()
        {
            return AppendAbsoluteUri()
                .AppendUrlReferrer()
                .AppendUserAgent()
                .AppendUserHostAddress()
                .AppendFormData()
                .GetException();
        }

        private ExceptionWithDataBuilder AppendFormData()
        {
            if (_request.Form != null && _request.Form.HasKeys())
            {
                var fieldsToHide = ConfigUtils.FieldsToHide;

                foreach (string key in _request.Form.Keys)
                {
                    string value;

                    // Prevent passwords to be shown
                    if (fieldsToHide.Contains(key.ToLowerInvariant()))
                    {
                        value = Convert.ToString(_request.Form[key]).Length > 0
                            ? "[hidden]"
                            : "[would be hidden but empty]";
                    }
                    else
                    {
                        value = Convert.ToString(_request.Form[key]);
                    }

                    _currentException.Data.Add($"form:{key}", value);
                }
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendUserHostAddress()
        {
            if (_request.UserHostAddress != null)
            {
                _currentException.Data.Add("UserHostAddress", _request.UserHostAddress);
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendUserAgent()
        {
            if (_request.UserAgent != null)
            {
                _currentException.Data.Add("UserAgent", _request.UserAgent);
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendAbsoluteUri()
        {
            if (!string.IsNullOrEmpty(_request.Url?.AbsoluteUri))
            {
                _currentException.Data.Add("AbsoluteUri", _request.Url.AbsoluteUri);
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendUrlReferrer()
        {
            if (!string.IsNullOrEmpty(_request.UrlReferrer?.AbsoluteUri))
            {
                _currentException.Data.Add("UrlReferrer", _request.UrlReferrer);
            }

            return this;
        }

        private Exception GetException()
        {
            return _currentException;
        }
    }
}