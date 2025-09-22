using Books_Core.Models;
using Microsoft.Extensions.Configuration;
using Xunit;
using System.Collections.Generic;

namespace BooksCore.Tests;

public class ConfigurationTests
{
    [Fact]
    public void BookstoreDatabaseSettings_Binds_FromConfiguration()
    {
        var inMemory = new Dictionary<string, string?> 
        {
            ["BookstoreDatabaseSettings:ConnectionString"] = "mongodb://localhost:27017",
            ["BookstoreDatabaseSettings:DatabaseName"] = "TestDb",
            ["BookstoreDatabaseSettings:BooksCollectionName"] = "Books"
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();

        // Option 1: Bind()
        var settings = new BookstoreDatabaseSettings();
        config.GetSection("BookstoreDatabaseSettings").Bind(settings);

        // Option 2: Cleaner alternative (you can use this instead of Bind)
        // var settings = config.GetSection("BookstoreDatabaseSettings").Get<BookstoreDatabaseSettings>();

        Assert.Equal("TestDb", settings.DatabaseName);
        Assert.Equal("Books", settings.BooksCollectionName);
        Assert.Equal("mongodb://localhost:27017", settings.ConnectionString);
    }
}
