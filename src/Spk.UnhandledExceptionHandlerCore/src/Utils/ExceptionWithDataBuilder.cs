using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Spk.Common.Helpers.Guard;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilder
    {
        private readonly Exception _exception;
        private readonly HttpRequestWrapper _request;
        private readonly HttpSessionState _session;

        public ExceptionWithDataBuilder(
            Exception exception,
            HttpRequestWrapper request,
            HttpSessionState session)
        {
            exception.GuardIsNotNull(nameof(exception));

            _request = request;
            _session = session;
            _exception = exception;
        }

        public Exception Build()
        {
            return AppendAbsoluteUri()
                .AppendUrlReferrer()
                .AppendUserAgent()
                .AppendUserHostAddress()
                .AppendFormData()
                .AppendSessionData()
                .GetException();
        }

        private ExceptionWithDataBuilder AppendSessionData()
        {
            if (_session != null && _session.Keys.Count > 0)
            {
                foreach (string key in _session.Keys)
                {
                    var value = _session[key];

                    if (value is IList)
                    {
                        var valueAsList = value as IList;
                        foreach (var enumValue in valueAsList)
                        {
                            var index = valueAsList.IndexOf(enumValue);
                            _exception.Data.Add($"session:{key}[{index}]", enumValue);
                        }
                    }
                    else
                    {
                        _exception.Data.Add($"session:{key}", value);
                    }
                }
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendFormData()
        {
            if (_request?.Form != null && _request.Form.HasKeys())
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

                    _exception.Data.Add($"form:{key}", value);
                }
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendUserHostAddress()
        {
            if (_request?.UserHostAddress != null)
            {
                _exception.Data.Add("UserHostAddress", _request.UserHostAddress);
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendUserAgent()
        {
            if (_request?.UserAgent != null)
            {
                _exception.Data.Add("UserAgent", _request.UserAgent);
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendAbsoluteUri()
        {
            if (!string.IsNullOrEmpty(_request?.Url?.AbsoluteUri))
            {
                _exception.Data.Add("AbsoluteUri", _request.Url.AbsoluteUri);
            }

            return this;
        }

        private ExceptionWithDataBuilder AppendUrlReferrer()
        {
            if (!string.IsNullOrEmpty(_request?.UrlReferrer?.AbsoluteUri))
            {
                _exception.Data.Add("UrlReferrer", _request.UrlReferrer);
            }

            return this;
        }

        private Exception GetException()
        {
            return _exception;
        }
    }
}