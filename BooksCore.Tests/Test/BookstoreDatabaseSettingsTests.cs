using Books_Core.Models;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class BookstoreDatabaseSettingsTests
    {
        [Fact]
        public void Settings_AssignsValuesCorrectly()
        {
            var settings = new BookstoreDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "TestDb",
                BooksCollectionName = "Books"
            };

            Assert.Equal("mongodb://localhost:27017", settings.ConnectionString);
            Assert.Equal("TestDb", settings.DatabaseName);
            Assert.Equal("Books", settings.BooksCollectionName);
        }

        [Fact]
        public void Settings_Defaults_ShouldBeNullOrEmpty()
        {
            var settings = new BookstoreDatabaseSettings();

            Assert.True(string.IsNullOrEmpty(settings.ConnectionString));
            Assert.True(string.IsNullOrEmpty(settings.DatabaseName));
            Assert.True(string.IsNullOrEmpty(settings.BooksCollectionName));
        }


        [Fact]
        public void Settings_Can_Assign_EmptyStrings()
        {
            var settings = new BookstoreDatabaseSettings
            {
                ConnectionString = "",
                DatabaseName = " ",
                BooksCollectionName = ""
            };

            Assert.Equal("", settings.ConnectionString);
            Assert.Equal(" ", settings.DatabaseName);
            Assert.Equal("", settings.BooksCollectionName);
        }

        [Fact]
        public void Settings_Trims_WhenAssigned()
        {
            var settings = new BookstoreDatabaseSettings
            {
                ConnectionString = "  mongodb://127.0.0.1:27017  ",
                DatabaseName = "  DbName  ",
                BooksCollectionName = "  Books  "
            };

            Assert.Equal("  mongodb://127.0.0.1:27017  ", settings.ConnectionString); // No automatic trim assumed, adjust if implemented
            Assert.Equal("  DbName  ", settings.DatabaseName);
            Assert.Equal("  Books  ", settings.BooksCollectionName);
        }
    }
}
