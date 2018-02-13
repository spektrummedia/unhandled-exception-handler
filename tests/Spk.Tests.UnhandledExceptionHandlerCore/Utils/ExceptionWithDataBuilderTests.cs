using System;
using System.Web;
using Moq;
using Shouldly;
using Spk.UnhandledExceptionHandlerCore.Utils;
using Xunit;

namespace Spk.Tests.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilderTests
    {
        public ExceptionWithDataBuilderTests()
        {
            _mockRequest = new Mock<HttpRequestWrapper>(new HttpRequest("", "http://localhost", ""));
            _exception = new Exception();
        }

        private readonly Mock<HttpRequestWrapper> _mockRequest;
        private readonly Exception _exception;

        [Fact]
        public void Build_ShouldAppendAbsoluteUri()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.Url)
                .Returns(new Uri("http://localhost"));

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["AbsoluteUri"].ShouldNotBeNull();
        }

        [Fact]
        public void Build_ShouldAppendUrlReferrer()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.UrlReferrer)
                .Returns(new Uri("http://localhost"));

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UrlReferrer"].ShouldNotBeNull();
        }

        [Fact]
        public void Build_ShouldAppendUserAgent()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.UserAgent)
                .Returns("test_useragent");

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UserAgent"].ShouldBe("test_useragent");
        }

        [Fact]
        public void Build_ShouldNotAppendAbsoluteUri_WhenNone()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["AbsoluteUri"].ShouldBeNull();
        }

        [Fact]
        public void Build_ShouldNotAppendUrlReferrer_WhenNone()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UrlReferrer"].ShouldBeNull();
        }

        [Fact]
        public void Build_ShouldAppendUserHostAddress()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.UserHostAddress)
                .Returns("test_hostaddress");

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UserHostAddress"].ShouldBe("test_hostaddress");
        }

        [Fact]
        public void Build_ShouldNotReturnNull()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object);

            // Act
            var result = builder.Build();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeSameAs(_exception);
        }

        [Fact]
        public void Build_ShouldNotUserAgent_WhenNone()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UserAgent"].ShouldBeNull();
        }

        [Fact]
        public void ctor_ShouldThrow_WhenExceptionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionWithDataBuilder(null, _mockRequest.Object));
        }

        [Fact]
        public void ctor_ShouldThrow_WhenRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionWithDataBuilder(_exception, null));
        }
    }
}