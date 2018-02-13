using System;
using System.Web;
using Spk.Common.Helpers.Guard;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilder
    {
        private Exception _currentException { get; }

        public ExceptionWithDataBuilder(
            Exception exception,
            HttpRequest request)
        {
            exception.GuardIsNotNull(nameof(exception));
            request.GuardIsNotNull(nameof(request));

            _currentException = exception;
        }

        public Exception Build()
        {
            return _currentException;
        }
    }
}