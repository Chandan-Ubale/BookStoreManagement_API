using Books_Core.Logger;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class SimpleFileLoggerTests : IDisposable
    {
        private readonly string tempPath;

        public SimpleFileLoggerTests()
        {
            tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.log");
        }

        [Fact]
        public void Log_WritesMessageToFile()
        {
            var logger = new SimpleFileLogger("TestCategory", tempPath);
            logger.Log(LogLevel.Information, new EventId(1), "Hello unit test", null, (s, e) => s.ToString());
            Assert.True(File.Exists(tempPath));
            var content = File.ReadAllText(tempPath);
            Assert.Contains("Hello unit test", content);
            Assert.Contains("TestCategory", content);
        }

        [Fact]
        public void IsEnabled_ShouldRespectLogLevel()
        {
            var logger = new SimpleFileLogger("TestCategory", tempPath);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.False(logger.IsEnabled(LogLevel.Debug));
        }

        [Fact]
        public void Log_ShouldHandleNullMessageWithoutException()
        {
            var logger = new SimpleFileLogger("TestCategory", tempPath);
            var exception = Record.Exception(() =>
                logger.Log<object>(LogLevel.Information, new EventId(), null, null, (s, e) => s == null ? string.Empty : s.ToString()));
            Assert.Null(exception);
        }

        [Fact]
        public void Log_ShouldHandleEmptyMessageWithoutException()
        {
            var logger = new SimpleFileLogger("TestCategory", tempPath);
            var exception = Record.Exception(() =>
                logger.Log<object>(LogLevel.Information, new EventId(), "", null, (s, e) => s == null ? string.Empty : s.ToString()));
            Assert.Null(exception);
        }

        public void Dispose()
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
}
