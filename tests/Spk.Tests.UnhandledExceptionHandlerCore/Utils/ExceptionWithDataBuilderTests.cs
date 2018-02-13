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
            _exception = new Exception();
        }

        private readonly HttpRequest _request;
        private readonly Exception _exception;

        [Fact]
        public void Build_ShouldAppendAbsoluteUri()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _request);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["AbsoluteUri"].ShouldNotBeNull();
        }

        [Fact]
        public void Build_ShouldNotReturnNull()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _request);

            // Act
            var result = builder.Build();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeSameAs(_exception);
        }

        [Fact]
        public void ctor_ShouldThrow_WhenExceptionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionWithDataBuilder(null, _request));
        }

        [Fact]
        public void ctor_ShouldThrow_WhenRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionWithDataBuilder(_exception, null));
        }
    }
}