using Books_Core.Logger;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class SimpleFileLoggerProviderTests : IDisposable
    {
        private readonly string tempPath;

        public SimpleFileLoggerProviderTests()
        {
            tempPath = Path.Combine(Path.GetTempPath(), $"testlog_{Guid.NewGuid()}.log");
        }

        [Fact]
        public void CreateLogger_ShouldReturnSimpleFileLogger()
        {
            var provider = new SimpleFileLoggerProvider(tempPath);
            var logger = provider.CreateLogger("TestCategory");
            Assert.NotNull(logger);
            Assert.IsType<SimpleFileLogger>(logger);
        }

        [Fact]
        public void Logger_IsEnabled_ShouldReturnTrueForInformation()
        {
            var provider = new SimpleFileLoggerProvider(tempPath);
            var logger = provider.CreateLogger("TestCategory");
            Assert.True(logger.IsEnabled(LogLevel.Information));
        }

        [Fact]
        public void Logger_IsEnabled_ShouldReturnFalseForDebug()
        {
            var provider = new SimpleFileLoggerProvider(tempPath);
            var logger = provider.CreateLogger("TestCategory");
            Assert.False(logger.IsEnabled(LogLevel.Debug));
        }

        [Fact]
        public void Logger_Log_WritesToFile()
        {
            var provider = new SimpleFileLoggerProvider(tempPath);
            var logger = provider.CreateLogger("TestCategory");

            logger.Log(LogLevel.Information, new EventId(1), "Hello unit test", null, (state, ex) => state.ToString());

            Assert.True(File.Exists(tempPath));
            var content = File.ReadAllText(tempPath);
            Assert.Contains("Hello unit test", content);
            Assert.Contains("TestCategory", content);
        }

        public void Dispose()
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
