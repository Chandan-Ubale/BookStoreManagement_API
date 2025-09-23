using Books_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class BookstoreDatabaseSettingsTests
    {
        [Fact]
        public void BookstoreDatabaseSettings_DefaultProperties_NotNull()
        {
            // Arrange & Act
            var settings = new BookstoreDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "TestDb",
                BooksCollectionName = "Books"
            };

            // Assert
            Assert.NotNull(settings.ConnectionString);
            Assert.NotNull(settings.DatabaseName);
            Assert.NotNull(settings.BooksCollectionName);
        }
    }
}
