using System;
using System.Web;
using Shouldly;
using Spk.UnhandledExceptionHandlerCore.Utils;
using Xunit;

namespace Spk.Tests.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilderTests
    {
        public ExceptionWithDataBuilderTests()
        {
            _request = new HttpRequest("", "http://localhost", "");
        }

        private readonly HttpRequest _request;

        [Fact]
        public void Build_ShouldNotReturnNull()
        {
            // Arrange
            var exception = new Exception();
            var builder = new ExceptionWithDataBuilder(exception, _request);

            // Act
            var result = builder.Build();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeSameAs(exception);
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