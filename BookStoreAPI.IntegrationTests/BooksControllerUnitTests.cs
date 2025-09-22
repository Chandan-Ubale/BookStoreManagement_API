using Xunit;
using Moq;
using Books_Core.Interface;
using Books_Core.Models;
using BookStoreAPI.Controllers;    
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.IntegrationTests
{
    public class BooksControllerUnitTests
    {
        private readonly Mock<IBookServices> _mockService;
        private readonly BooksController _controller;

        public BooksControllerUnitTests()
        {
            _mockService = new Mock<IBookServices>();
            _controller = new BooksController(_mockService.Object);
        }

        [Fact]
        public void Get_ReturnsList()
        {
            // Arrange
            var data = new List<Books> { new Books { Title = "T" } };
            _mockService.Setup(s => s.GetAllBooks()).Returns(data);

            // Act
            var actionResult = _controller.Get();

            // Assert: ActionResult<T> when returned as T sets .Value
            Assert.NotNull(actionResult.Value);
            Assert.Single(actionResult.Value);
            Assert.Equal("T", actionResult.Value[0].Title);
        }

        [Fact]
        public void GetById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetBookById("1")).Returns((Books?)null);

            var res = _controller.Get("1");
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void Create_ReturnsCreatedAtRoute()
        {
            var book = new Books { Id = "1", Title = "X" };
            _mockService.Setup(s => s.AddBook(book)).Verifiable();

            var res = _controller.Create(book);
            var created = Assert.IsType<CreatedAtRouteResult>(res.Result);
            Assert.Equal("GetBook", created.RouteName);
        }
    }
}