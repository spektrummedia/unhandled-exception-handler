using System;
using System.Web;
using Spk.Common.Helpers.Guard;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilder
    {
        private readonly Exception _currentException;
        private readonly HttpRequest _request;

        public ExceptionWithDataBuilder(
            Exception exception,
            HttpRequest request)
        {
            exception.GuardIsNotNull(nameof(exception));
            request.GuardIsNotNull(nameof(request));

            _request = request;
            _currentException = exception;
        }

        public Exception Build()
        {
            return AppendAbsoluteUri();
        }

        private Exception AppendAbsoluteUri()
        {
            if (!string.IsNullOrEmpty(_request.Url.AbsoluteUri))
            {
                _currentException.Data.Add("AbsoluteUri", _request.Url.AbsoluteUri);
            }

            return _currentException;
        }
    }
}