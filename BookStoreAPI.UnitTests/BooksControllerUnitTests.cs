using Xunit;
using Moq;
using Books_Core.Interface;
using Books_Core.Models;
using Books_Core.Dtos;
using BookStoreAPI.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BookStoreAPI.UnitTests
{
    public class BooksControllerUnitTests
    {
        private readonly Mock<IBookServices> mockService;
        private readonly BooksController controller;

        public BooksControllerUnitTests()
        {
            mockService = new Mock<IBookServices>();
            var logger = new LoggerFactory().CreateLogger<BooksController>();
            controller = new BooksController(mockService.Object, logger);
        }

        // --------- GET all books ---------

        [Fact]
        public void Get_ReturnsNotFound_WhenNoBooks()
        {
            mockService.Setup(s => s.GetAllBooks()).Returns(new List<Books>());
            var result = controller.Get();
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<Books>>>(notFound.Value);
            Assert.Equal(404, response.Status);
            Assert.Equal("No books available.", response.Message);
        }

        [Fact]
        public void Get_ReturnsBooks_WhenBooksExist()
        {
            var books = new List<Books> { new Books { Id = "1", Title = "Title1" } };
            mockService.Setup(s => s.GetAllBooks()).Returns(books);

            var result = controller.Get();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<Books>>>(okResult.Value);
            Assert.Equal(200, response.Status);
            Assert.Equal("1 books retrieved successfully.", response.Message);
            Assert.Single(response.Data);
        }

        // --------- GET book by ID ---------

        [Fact]
        public void Get_ById_ReturnsBadRequest_WhenIdIsEmpty()
        {
            var result = controller.Get("");
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<Books>>(badRequest.Value);
            Assert.Equal(400, response.Status);
            Assert.Equal("Book ID cannot be null or empty.", response.Message);
        }

        [Fact]
        public void Get_ById_ReturnsNotFound_WhenBookMissing()
        {
            mockService.Setup(s => s.GetBookById("99")).Returns((Books)null);
            var result = controller.Get("99");
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<Books>>(notFound.Value);
            Assert.Equal(404, response.Status);
            Assert.Equal("Book not found.", response.Message);
        }

        [Fact]
        public void Get_ById_ReturnsOk_WhenBookExists()
        {
            var book = new Books { Id = "1", Title = "Sample" };
            mockService.Setup(s => s.GetBookById("1")).Returns(book);

            var result = controller.Get("1");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<Books>>(okResult.Value);
            Assert.Equal(200, response.Status);
            Assert.Equal("Book retrieved successfully.", response.Message);
            Assert.Equal("Sample", response.Data.Title);
        }

        // --------- Create ---------

        [Fact]
        public void Create_ReturnsBadRequest_WhenBookIsNull()
        {
            var result = controller.Create(null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<Books>>(badRequest.Value);
            Assert.Equal(400, response.Status);
            Assert.Equal("Invalid book data.", response.Message);
        }

        [Fact]
        public void Create_ReturnsCreated_WhenBookIsValid()
        {
            var book = new Books { Id = "1", Title = "New Book" };
            mockService.Setup(s => s.AddBook(book));
            var result = controller.Create(book);

            var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
            var response = Assert.IsType<ApiResponse<Books>>(created.Value);
            Assert.Equal(201, response.Status);
            Assert.Equal("Book created successfully.", response.Message);
            Assert.Equal("New Book", response.Data.Title);
        }

        // --------- Bulk create ---------

        [Fact]
        public void CreateBulk_ReturnsBadRequest_WhenListEmpty()
        {
            var result = controller.CreateBulk(new List<Books>());
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<Books>>>(badRequest.Value);
            Assert.Equal(400, response.Status);
            Assert.Equal("Invalid bulk book data.", response.Message);
        }

        [Fact]
        public void CreateBulk_ReturnsCreated_WhenListValid()
        {
            var books = new List<Books> { new Books { Id = "1", Title = "First" }, new Books { Id = "2", Title = "Second" } };
            mockService.Setup(s => s.AddBooksBulk(books));
            var result = controller.CreateBulk(books);
            var created = Assert.IsType<CreatedResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<Books>>>(created.Value);
            Assert.Equal(201, response.Status);
            Assert.Equal("2 books added successfully.", response.Message);
        }

        // --------- Update ---------

        [Fact]
        public void Update_ReturnsBadRequest_WhenIdEmptyOrBookNull()
        {
            var result = controller.Update("", null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Equal(400, response.Status);
            Assert.Equal("Invalid update request.", response.Message);
        }

        [Fact]
        public void Update_ReturnsNotFound_WhenBookMissing()
        {
            mockService.Setup(s => s.UpdateBook("99", It.IsAny<Books>())).Throws<KeyNotFoundException>();
            var book = new Books { Title = "Test" };
            var result = controller.Update("99", book);
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(notFound.Value);
            Assert.Equal(404, response.Status);
            Assert.Equal("Book not found.", response.Message);
        }

        [Fact]
        public void Update_ReturnsBadRequest_WhenArgumentException()
        {
            mockService.Setup(s => s.UpdateBook("1", It.IsAny<Books>())).Throws(new System.ArgumentException("Invalid argument"));
            var book = new Books { Title = "Test" };
            var result = controller.Update("1", book);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Equal(400, response.Status);
            Assert.Equal("Invalid argument", response.Message);
        }

        [Fact]
        public void Update_ReturnsOk_WhenSuccess()
        {
            var book = new Books { Title = "Updated" };
            mockService.Setup(s => s.UpdateBook("1", book));
            var result = controller.Update("1", book);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(ok.Value);
            Assert.Equal(200, response.Status);
            Assert.Equal("Book updated successfully.", response.Message);
        }

        // --------- Patch ---------

        [Fact]
        public void Patch_ReturnsBadRequest_WhenIdEmptyOrPatchDtoNull()
        {
            var result = controller.Patch("", null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Equal(400, response.Status);
            Assert.Equal("Invalid patch request.", response.Message);
        }

        [Fact]
        public void Patch_ReturnsNotFound_WhenBookMissing()
        {
            mockService.Setup(s => s.PatchBook("99", It.IsAny<BookPatchDto>())).Throws<KeyNotFoundException>();
            var patchDto = new BookPatchDto { Title = "New title" };
            var result = controller.Patch("99", patchDto);
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(notFound.Value);
            Assert.Equal(404, response.Status);
            Assert.Equal("Book not found.", response.Message);
        }

        [Fact]
        public void Patch_ReturnsBadRequest_WhenArgumentException()
        {
            mockService.Setup(s => s.PatchBook("1", It.IsAny<BookPatchDto>())).Throws(new System.ArgumentException("Invalid argument"));
            var patchDto = new BookPatchDto { Title = "New title" };
            var result = controller.Patch("1", patchDto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Equal(400, response.Status);
            Assert.Equal("Invalid argument", response.Message);
        }

        [Fact]
        public void Patch_ReturnsOk_WhenSuccess()
        {
            var patchDto = new BookPatchDto { Title = "Patched title" };
            mockService.Setup(s => s.PatchBook("1", patchDto));
            var result = controller.Patch("1", patchDto);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(ok.Value);
            Assert.Equal(200, response.Status);
            Assert.Equal("Book patched successfully.", response.Message);
        }

        // --------- Delete ---------

        [Fact]
        public void Delete_ReturnsBadRequest_WhenIdEmpty()
        {
            var result = controller.Delete("");
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
            Assert.Equal(400, response.Status);
            Assert.Equal("Book ID cannot be null or empty.", response.Message);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenBookMissing()
        {
            mockService.Setup(s => s.DeleteBook("99")).Throws<KeyNotFoundException>();
            var result = controller.Delete("99");
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(notFound.Value);
            Assert.Equal(404, response.Status);
            Assert.Equal("Book not found.", response.Message);
        }

        [Fact]
        public void Delete_ReturnsOk_WhenSuccess()
        {
            mockService.Setup(s => s.DeleteBook("1"));
            var result = controller.Delete("1");
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<object>>(ok.Value);
            Assert.Equal(200, response.Status);
            Assert.Equal("Book deleted successfully.", response.Message);
        }
    }
}
