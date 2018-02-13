using Shouldly;
using Xunit;

namespace Spk.Tests.UnhandledExceptionHandlerCore.Utils
{
    public class ExceptionWithDataBuilderTests
    {
        [Fact]
        public void Test_ShouldPass()
        {
            true.ShouldBeTrue();
        }
    }
}