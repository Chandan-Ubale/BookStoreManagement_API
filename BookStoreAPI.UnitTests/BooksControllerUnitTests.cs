using Xunit;
using Moq;
using Books_Core.Interface;
using Books_Core.Models;
using BookStoreAPI.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace BookStoreAPI.UnitTests
{
    public class BooksControllerUnitTests
    {
        private readonly Mock<IBookServices> _mockService;
        private readonly BooksController _controller;

        public BooksControllerUnitTests()
        {
            _mockService = new Mock<IBookServices>();
            var logger = NullLogger<BooksController>.Instance;
            _controller = new BooksController(_mockService.Object, logger);
        }

        [Fact]
        public void Get_ReturnsListOfBooks()
        {
            var sampleBooks = new List<Books>
            {
                new Books { Id = "1", Title = "Book1", Author = "Author1", Price = 10M },
                new Books { Id = "2", Title = "Book2", Author = "Author2", Price = 20M }
            };
            _mockService.Setup(s => s.GetAllBooks()).Returns(sampleBooks);

            var actionResult = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var books = Assert.IsType<List<Books>>(okResult.Value);
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public void UpdateBook_NotFound_ReturnsNotFound()
        {
            var book = new Books { Title = "Updated" };
            var id = "1";
            _mockService.Setup(s => s.UpdateBook(id, book)).Throws<KeyNotFoundException>();

            var result = _controller.Update(id, book);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateBook_InvalidData_ReturnsBadRequest()
        {
            var book = new Books { Title = "" };
            var id = "1";
            _mockService.Setup(s => s.UpdateBook(id, book)).Throws<ArgumentException>();

            var result = _controller.Update(id, book);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteBook_NotFound_ReturnsNotFound()
        {
            var id = "1";
            _mockService.Setup(s => s.DeleteBook(id)).Throws<KeyNotFoundException>();

            var result = _controller.Delete(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetById_ReturnsBook_WhenExists()
        {
            var book = new Books { Id = "1", Title = "Book1" };
            _mockService.Setup(s => s.GetBookById("1")).Returns(book);

            var actionResult = _controller.Get("1");

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedBook = Assert.IsType<Books>(okResult.Value);
            Assert.Equal("Book1", returnedBook.Title);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenNotExists()
        {
            _mockService.Setup(s => s.GetBookById("999")).Returns((Books?)null);

            var actionResult = _controller.Get("999");

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public void GetById_NullId_ReturnsBadRequest()
        {
            var result = _controller.Get(null!);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void Create_ReturnsCreatedAtRoute()
        {
            var book = new Books { Id = "1", Title = "New Book" };
            _mockService.Setup(s => s.AddBook(book));

            var actionResult = _controller.Create(book);

            var createdResult = Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
            Assert.Equal("GetBook", createdResult.RouteName);
        }

        [Fact]
        public void Create_NullBook_ReturnsBadRequest()
        {
            var result = _controller.Create(null!);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void CreateBulk_ReturnsCreatedResult()
        {
            var books = new List<Books> { new Books { Title = "B1" }, new Books { Title = "B2" } };
            _mockService.Setup(s => s.AddBooksBulk(books));

            var result = _controller.CreateBulk(books);

            var created = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(books, created.Value);
        }

        [Fact]
        public void CreateBulk_EmptyList_ReturnsBadRequest()
        {
            var result = _controller.CreateBulk(new List<Books>());
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void CreateBulk_NullList_ReturnsBadRequest()
        {
            var result = _controller.CreateBulk(null!);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateBook_ReturnsNoContent()
        {
            var book = new Books { Title = "Updated" };
            var id = "1";

            _mockService.Setup(s => s.UpdateBook(id, book));

            var result = _controller.Update(id, book);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateBook_NullBook_ReturnsBadRequest()
        {
            var result = _controller.Update("1", null!);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateBook_NullId_ReturnsBadRequest()
        {
            var book = new Books { Title = "Updated" };
            var result = _controller.Update(null!, book);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteBook_ReturnsNoContent()
        {
            var id = "1";
            _mockService.Setup(s => s.DeleteBook(id));

            var result = _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteBook_NullId_ReturnsBadRequest()
        {
            var result = _controller.Delete(null!);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
