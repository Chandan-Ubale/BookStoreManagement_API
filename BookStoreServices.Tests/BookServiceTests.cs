using Books_Core.Dtos;
using Books_Core.Interface;
using Books_Core.Models;
using Books_Services.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> mockRepo;
    private readonly Mock<ILogger<BookService>> mockLogger;
    private readonly BookService service;

    public BookServiceTests()
    {
        mockRepo = new Mock<IBookRepository>();
        mockLogger = new Mock<ILogger<BookService>>();
        service = new BookService(mockRepo.Object, mockLogger.Object);
    }

    [Fact]
    public void GetBookById_ValidId_ReturnsBook()
    {
        var book = new Books { Id = "1", Title = "A", Author = "B", Price = 99 };
        mockRepo.Setup(r => r.GetBookById("1")).Returns(book);
        var result = service.GetBookById("1");

        Assert.NotNull(result);
        Assert.Equal("A", result.Title);
    }

    [Fact]
    public void PatchBook_NullPatchDto_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => service.PatchBook("1", null));
    }

    [Fact]
    public void PatchBook_NonExisting_ThrowsKeyNotFoundException()
    {
        mockRepo.Setup(r => r.GetBookById("99")).Returns((Books)null);
        var patch = new BookPatchDto { Title = "New" };

        Assert.Throws<KeyNotFoundException>(() => service.PatchBook("99", patch));
    }

    [Fact]
    public void PatchBook_ValidUpdate_UpdatesRepo()
    {
        var book = new Books { Id = "1", Title = "Old", Price = 1 };
        mockRepo.Setup(r => r.GetBookById("1")).Returns(book);

        Books? updatedBook = null;
        mockRepo.Setup(r => r.UpdateBook("1", It.IsAny<Books>()))
            .Callback<string, Books>((id, b) => updatedBook = b);

        var patch = new BookPatchDto { Title = "New", Price = 22 };
        service.PatchBook("1", patch);

        Assert.NotNull(updatedBook);
        Assert.Equal("New", updatedBook.Title);
        Assert.Equal(22, updatedBook.Price);
    }

    [Fact]
    public void GetBookById_NonExisting_ReturnsNull()
    {
        mockRepo.Setup(r => r.GetBookById("42")).Returns((Books)null);
        var result = service.GetBookById("42");

        Assert.Null(result);
    }

    [Fact]
    public void AddBook_CallsRepository()
    {
        var book = new Books { Id = "1", Title = "New Book", Author = "Author", Price = 100 };
        service.AddBook(book);
        mockRepo.Verify(r => r.AddBook(book), Times.Once);
    }

    [Fact]
    public void UpdateBook_BookNotFound_ThrowsKeyNotFoundException()
    {
        mockRepo.Setup(r => r.GetBookById("1")).Returns((Books)null);
        var book = new Books { Title = "Updated" };
        Assert.Throws<KeyNotFoundException>(() => service.UpdateBook("1", book));
    }

    [Fact]
    public void DeleteBook_BookNotFound_ThrowsKeyNotFoundException()
    {
        mockRepo.Setup(r => r.GetBookById("1")).Returns((Books)null);
        Assert.Throws<KeyNotFoundException>(() => service.DeleteBook("1"));
    }


    [Fact]
    public void UpdateBook_ValidBook_CallsUpdate()
    {
        var book = new Books { Id = "1", Title = "Old Title", Author = "A", Price = 10M };
        mockRepo.Setup(r => r.GetBookById("1")).Returns(book);

        var updatedBook = new Books { Id = "1", Title = "New Title", Author = "A", Price = 12M };
        service.UpdateBook("1", updatedBook);

        mockRepo.Verify(r => r.UpdateBook("1", updatedBook), Times.Once);
    }

    

    [Fact]
    public void DeleteBook_ValidCall_CallsDelete()
    {
        var book = new Books { Id = "1", Title = "To delete" };
        mockRepo.Setup(r => r.GetBookById("1")).Returns(book);

        service.DeleteBook("1");

        mockRepo.Verify(r => r.DeleteBook("1"), Times.Once);
    }
}
