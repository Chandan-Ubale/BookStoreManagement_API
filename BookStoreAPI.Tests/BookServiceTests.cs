using Books_Core.Interface;
using Books_Core.Models;
using Books_Services.Services;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace BookStoreServices.Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockRepo;
        private readonly BookService _service;

        public BookServiceTests()
        {
            _mockRepo = new Mock<IBookRepository>();
            _service = new BookService(_mockRepo.Object);
        }

        [Fact]
        public void GetAllBooks_ReturnsAllBooks()
        {
            // Arrange
            var books = new List<Books>
            {
                new Books { Id = "1", Title = "Book 1", Author = "Author 1", Price = 100 },
                new Books { Id = "2", Title = "Book 2", Author = "Author 2", Price = 200 }
            };
            _mockRepo.Setup(r => r.GetAllBooks()).Returns(books);

            // Act
            var result = _service.GetAllBooks();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Book 1", result[0].Title);
        }

        [Fact]
        public void GetBookById_ExistingId_ReturnsBook()
        {
            // Arrange
            var book = new Books { Id = "1", Title = "Book 1", Author = "Author 1", Price = 100 };
            _mockRepo.Setup(r => r.GetBookById("1")).Returns(book);

            // Act
            var result = _service.GetBookById("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Book 1", result.Title);
        }

        [Fact]
        public void GetBookById_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetBookById("99")).Returns((Books?)null);

            // Act
            var result = _service.GetBookById("99");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddBook_CallsRepositoryOnce()
        {
            // Arrange
            var book = new Books { Id = "1", Title = "Book 1", Author = "Author 1", Price = 100 };

            // Act
            _service.AddBook(book);

            // Assert
            _mockRepo.Verify(r => r.AddBook(book), Times.Once);
        }

        [Fact]
        public void AddBooksBulk_CallsRepositoryOnce()
        {
            // Arrange
            var books = new List<Books>
            {
                new Books { Id = "1", Title = "Book 1", Author = "Author 1", Price = 100 },
                new Books { Id = "2", Title = "Book 2", Author = "Author 2", Price = 200 }
            };

            // Act
            _service.AddBooksBulk(books);

            // Assert
            _mockRepo.Verify(r => r.AddBooksBulk(books), Times.Once);
        }

        [Fact]
        public void UpdateBook_ValidBook_CallsRepository()
        {
            // Arrange
            var book = new Books { Id = "1", Title = "Updated", Author = "Author 1", Price = 150 };

            // Act
            _service.UpdateBook("1", book);

            // Assert
            _mockRepo.Verify(r => r.UpdateBook("1", book), Times.Once);
        }

        [Fact]
        public void DeleteBook_ExistingId_CallsRepository()
        {
            // Act
            _service.DeleteBook("1");

            // Assert
            _mockRepo.Verify(r => r.DeleteBook("1"), Times.Once);
        }
    }
}
