using System;
using System.Web;
using Moq;
using Spk.UnhandledExceptionHandlerCore.Utils;
using Xunit;

namespace Spk.Tests.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilderTests
    {
        private HttpRequest _request;

        public ExceptionWithDataBuilderTests()
        {
            _request = new HttpRequest("", "http://localhost", "");
        }

        [Fact]
        public void ctor_ShouldThrow_WhenExceptionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionWithDataBuilder(null, _request));
        }

        [Fact]
        public void ctor_ShouldThrow_WhenRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionWithDataBuilder(new Exception(), null));
        }
    }
}