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

        #region GetAllBooks

        [Fact]
        public void GetAllBooks_ReturnsAllBooks()
        {
            var books = new List<Books>
            {
                new Books { Id = "1", Title = "Book 1", Author = "Author 1", Price = 100 },
                new Books { Id = "2", Title = "Book 2", Author = "Author 2", Price = 200 }
            };
            _mockRepo.Setup(r => r.GetAllBooks()).Returns(books);

            var result = _service.GetAllBooks();

            Assert.Equal(2, result.Count);
            Assert.Equal("Book 1", result[0].Title);
        }

        [Fact]
        public void GetAllBooks_EmptyList_ReturnsEmpty()
        {
            _mockRepo.Setup(r => r.GetAllBooks()).Returns(new List<Books>());

            var result = _service.GetAllBooks();

            Assert.Empty(result);
        }

        #endregion

        #region GetBookById

        [Fact]
        public void GetBookById_ExistingId_ReturnsBook()
        {
            var book = new Books { Id = "1", Title = "Book 1", Author = "Author 1", Price = 100 };
            _mockRepo.Setup(r => r.GetBookById("1")).Returns(book);

            var result = _service.GetBookById("1");

            Assert.NotNull(result);
            Assert.Equal("Book 1", result.Title);
        }

        [Fact]
        public void GetBookById_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetBookById("99")).Returns((Books?)null);

            var result = _service.GetBookById("99");

            Assert.Null(result);
        }

        [Fact]
        public void GetBookById_NullOrEmptyId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.GetBookById(null!));
            Assert.Throws<ArgumentNullException>(() => _service.GetBookById(""));
            Assert.Throws<ArgumentNullException>(() => _service.GetBookById(" "));
        }

        #endregion

        #region AddBook

        [Fact]
        public void AddBook_ValidBook_CallsRepository()
        {
            var book = new Books { Title = "Book 1", Author = "Author 1", Price = 100 };

            _service.AddBook(book);

            _mockRepo.Verify(r => r.AddBook(book), Times.Once);
        }

        [Fact]
        public void AddBook_NullBook_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.AddBook(null!));
        }

        [Fact]
        public void AddBook_InvalidBook_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.AddBook(new Books { Title = "", Author = "A", Price = 10 }));
            Assert.Throws<ArgumentException>(() => _service.AddBook(new Books { Title = "T", Author = "", Price = 10 }));
            Assert.Throws<ArgumentException>(() => _service.AddBook(new Books { Title = "T", Author = "A", Price = 0 }));
        }

        #endregion

        #region AddBooksBulk

        [Fact]
        public void AddBooksBulk_ValidList_CallsRepository()
        {
            var books = new List<Books>
            {
                new Books { Title = "B1", Author = "A1", Price = 10 },
                new Books { Title = "B2", Author = "A2", Price = 20 }
            };

            _service.AddBooksBulk(books);

            _mockRepo.Verify(r => r.AddBooksBulk(books), Times.Once);
        }

        [Fact]
        public void AddBooksBulk_NullOrEmpty_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _service.AddBooksBulk(null!));
            Assert.Throws<ArgumentException>(() => _service.AddBooksBulk(new List<Books>()));
        }

        [Fact]
        public void AddBooksBulk_InvalidBookInList_Throws()
        {
            var books = new List<Books>
            {
                new Books { Title = "", Author = "A", Price = 10 },
                new Books { Title = "B2", Author = "A2", Price = 0 }
            };
            Assert.Throws<ArgumentException>(() => _service.AddBooksBulk(books));
        }

        #endregion

        #region UpdateBook

        [Fact]
        public void UpdateBook_Valid_CallsRepository()
        {
            var book = new Books { Title = "Updated", Author = "A", Price = 50 };
            _service.UpdateBook("1", book);
            _mockRepo.Verify(r => r.UpdateBook("1", book), Times.Once);
        }

        [Fact]
        public void UpdateBook_NullBookOrId_Throws()
        {
            var book = new Books { Title = "T", Author = "A", Price = 10 };
            Assert.Throws<ArgumentNullException>(() => _service.UpdateBook(null!, book));
            Assert.Throws<ArgumentNullException>(() => _service.UpdateBook("1", null!));
        }

        [Fact]
        public void UpdateBook_InvalidBook_Throws()
        {
            Assert.Throws<ArgumentException>(() => _service.UpdateBook("1", new Books { Title = "", Author = "A", Price = 10 }));
            Assert.Throws<ArgumentException>(() => _service.UpdateBook("1", new Books { Title = "T", Author = "", Price = 10 }));
            Assert.Throws<ArgumentException>(() => _service.UpdateBook("1", new Books { Title = "T", Author = "A", Price = 0 }));
        }

        #endregion

        #region DeleteBook

        [Fact]
        public void DeleteBook_Valid_CallsRepository()
        {
            _service.DeleteBook("1");
            _mockRepo.Verify(r => r.DeleteBook("1"), Times.Once);
        }

        [Fact]
        public void DeleteBook_NullOrEmptyId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.DeleteBook(null!));
            Assert.Throws<ArgumentNullException>(() => _service.DeleteBook(""));
            Assert.Throws<ArgumentNullException>(() => _service.DeleteBook(" "));
        }

        [Fact]
        public void DeleteBook_RepositoryThrowsException_Propagates()
        {
            _mockRepo.Setup(r => r.DeleteBook("1")).Throws<InvalidOperationException>();
            Assert.Throws<InvalidOperationException>(() => _service.DeleteBook("1"));
        }

        #endregion
    }
}
