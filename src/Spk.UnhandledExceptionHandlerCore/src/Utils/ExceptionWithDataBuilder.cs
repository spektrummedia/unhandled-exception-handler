using System;
using System.Web;
using Spk.Common.Helpers.Guard;

namespace Spk.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilder
    {
        public ExceptionWithDataBuilder(
            Exception exception,
            HttpRequest request)
        {
            exception.GuardIsNotNull(nameof(exception));
            request.GuardIsNotNull(nameof(request));
        }
    }
}