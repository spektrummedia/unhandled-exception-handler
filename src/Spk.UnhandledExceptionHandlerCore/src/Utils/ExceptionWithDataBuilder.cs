using System;
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
                .GetException();
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