using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.SessionState;
using Moq;
using Shouldly;
using Spk.Tests.UnhandledExceptionHandlerCore.Mocks;
using Spk.UnhandledExceptionHandlerCore.Utils;
using Xunit;

namespace Spk.Tests.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilderTests
    {
        public ExceptionWithDataBuilderTests()
        {
            _mockRequest = new Mock<HttpRequestWrapper>(new HttpRequest("", "http://localhost", ""));
            _mockSessionState = FakeHttpSessionState.Build();
            _exception = new Exception();
        }

        private readonly Mock<HttpRequestWrapper> _mockRequest;
        private readonly Exception _exception;
        private readonly HttpSessionState _mockSessionState;

        [Fact]
        public void Build_ShouldAppendAbsoluteUri()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.Url)
                .Returns(new Uri("http://localhost"));

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["AbsoluteUri"].ShouldNotBeNull();
        }

        [Fact]
        public void Build_ShouldAppendFormData()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.Form)
                .Returns(new NameValueCollection
                {
                    {"key_1", "value_1"}
                });

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["form:key_1"].ShouldBe("value_1");
        }

        [Fact]
        public void Build_ShouldAppendSessionData_WhenIList()
        {
            // Arrange
            _mockSessionState.Add("key_1", new[] {"value_1", "value_2"});

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["session:key_1[0]"].ShouldBe("value_1");
            resul.Data["session:key_1[1]"].ShouldBe("value_2");
        }

        [Fact]
        public void Build_ShouldAppendSessionData_WhenPrimitive()
        {
            // Arrange
            _mockSessionState.Add("key_1", "value_1");

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["session:key_1"].ShouldBe("value_1");
        }

        [Fact]
        public void Build_ShouldAppendUrlReferrer()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.UrlReferrer)
                .Returns(new Uri("http://localhost"));

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

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

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UserAgent"].ShouldBe("test_useragent");
        }

        [Fact]
        public void Build_ShouldAppendUserHostAddress()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.UserHostAddress)
                .Returns("test_hostaddress");

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UserHostAddress"].ShouldBe("test_hostaddress");
        }

        [Fact]
        public void Build_ShouldNotAppendAbsoluteUri_WhenNone()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["AbsoluteUri"].ShouldBeNull();
        }

        [Fact]
        public void Build_ShouldNotAppendFormData_WhenEmpty()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.Form)
                .Returns(new NameValueCollection());

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data.Keys.Count.ShouldBe(0);
        }

        [Fact]
        public void Build_ShouldNotAppendFormData_WhenNull()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.Form)
                .Returns((NameValueCollection) null);

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data.Keys.Count.ShouldBe(0);
        }

        [Fact]
        public void Build_ShouldNotAppendFormData_WhenPassword()
        {
            // Arrange
            _mockRequest
                .Setup(x => x.Form)
                .Returns(new NameValueCollection
                {
                    {"password", "value_1"}
                });

            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["form:password"].ShouldBe("[hidden]");
        }

        [Fact]
        public void Build_ShouldNotAppendSessionData_WhenNull()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, null);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data.Keys.Count.ShouldBe(0);
        }

        [Fact]
        public void Build_ShouldNotAppendUrlReferrer_WhenNone()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UrlReferrer"].ShouldBeNull();
        }

        [Fact]
        public void Build_ShouldNotReturnNull()
        {
            // Arrange
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

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
            var builder = new ExceptionWithDataBuilder(_exception, _mockRequest.Object, _mockSessionState);

            // Act
            var resul = builder.Build();

            // Assert
            resul.Data["UserAgent"].ShouldBeNull();
        }

        [Fact]
        public void ctor_ShouldThrow_WhenExceptionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ExceptionWithDataBuilder(null, _mockRequest.Object, _mockSessionState));
        }

        [Fact]
        public void ctor_ShouldThrow_WhenRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ExceptionWithDataBuilder(_exception, null, _mockSessionState));
        }
    }
}