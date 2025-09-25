using Books_Core.Models;
using Books_Infrastructure.Data;
using Books_Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Xunit;

public class BookRepositoryUnitTest : IDisposable
{
    private readonly MongoDbRunner runner;
    private readonly BookRepository repo;

    public BookRepositoryUnitTest()
    {
        runner = MongoDbRunner.Start();
        var options = Options.Create(new BookstoreDatabaseSettings
        {
            ConnectionString = runner.ConnectionString,
            DatabaseName = "TestDb",
            BooksCollectionName = "Books"
        });
        var context = new MongoDbContext(options);
        repo = new BookRepository(context);
    }

    [Fact]
    public void AddAndGetBook_Works()
    {
        var book = new Books { Title = "X", Author = "Y", Price = 123 };
        repo.AddBook(book);
        var all = repo.GetAllBooks();
        Assert.Contains(all, b => b.Title == "X" && b.Author == "Y");
    }

    [Fact]
    public void GetBookById_ReturnsNull_WhenBookMissing()
    {
        var result = repo.GetBookById("507f1f77bcf86cd799439011");
        Assert.Null(result);
    }

    [Fact]
    public void UpdateBook_ThrowsKeyNotFound_WhenMissing()
    {
        var missing = new Books { Id = "507f1f77bcf86cd799439011", Title = "X", Author = "Y" };
        Assert.Throws<KeyNotFoundException>(() => repo.UpdateBook(missing.Id, missing));
    }

    [Fact]
    public void UpdateBook_UpdatesExistingBook()
    {
        var book = new Books { Title = "Orig", Author = "A", Price = 10M };
        repo.AddBook(book);
        book.Title = "Updated";
        repo.UpdateBook(book.Id, book);
        var fetched = repo.GetBookById(book.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Updated", fetched.Title);
    }

    [Fact]
    public void DeleteBook_ThrowsKeyNotFound_WhenMissing()
    {
        Assert.Throws<KeyNotFoundException>(() => repo.DeleteBook("507f1f77bcf86cd799439011"));
    }

    [Fact]
    public void DeleteBook_RemovesBook()
    {
        var book = new Books { Title = "Temp", Author = "A", Price = 1 };
        repo.AddBook(book);
        repo.DeleteBook(book.Id);
        var fetched = repo.GetBookById(book.Id);
        Assert.Null(fetched);
    }

    [Fact]
    public void GetAllBooks_ReturnsBooks()
    {
        var book1 = new Books { Title = "Book1", Author = "Author1", Price = 10 };
        var book2 = new Books { Title = "Book2", Author = "Author2", Price = 20 };
        repo.AddBook(book1);
        repo.AddBook(book2);
        var allBooks = repo.GetAllBooks();
        Assert.Contains(allBooks, b => b.Title == "Book1");
        Assert.Contains(allBooks, b => b.Title == "Book2");
    }

    public void Dispose() => runner.Dispose();
}
