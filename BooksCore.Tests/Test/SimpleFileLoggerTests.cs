using Books_Core.Logger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class SimpleFileLoggerTests
    {
        [Fact]
        public void Log_WritesMessageToFile()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".log");
            try
            {
                var logger = new SimpleFileLogger("TestCategory", tempPath);

                // Act
                logger.Log<string>(LogLevel.Information, new EventId(1), "Hello unit test", null, (s, e) => s);

                // Assert
                Assert.True(File.Exists(tempPath), "Log file should exist");
                var content = File.ReadAllText(tempPath);
                Assert.Contains("Hello unit test", content);
                Assert.Contains("TestCategory", content);
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }
    }
}
