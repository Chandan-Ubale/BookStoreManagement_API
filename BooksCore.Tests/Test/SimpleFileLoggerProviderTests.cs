using Books_Core.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class SimpleFileLoggerProviderTests
    {
        [Fact]
        public void CreateLogger_ReturnsSimpleFileLogger()
        {
            // Arrange
            var tempPath = "testlog.log";
            var provider = new SimpleFileLoggerProvider(tempPath);

            // Act
            var logger = provider.CreateLogger("TestCategory");

            // Assert
            Assert.NotNull(logger);
            Assert.IsType<SimpleFileLogger>(logger);
        }
    }
}
