using Books_Core.Interface;
using Books_Core.Models;
using Books_Core.Dtos; // unified DTOs in Core
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // JWT required for all endpoints
    public class BooksController : ControllerBase
    {
        private readonly IBookServices _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookServices bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // ---------------- READ ----------------
        [HttpGet]
        [SwaggerOperation(Summary = "Get all books")]
        [Authorize(Roles = "Admin,Moderator,ReadOnly")]
        public ActionResult<ApiResponse<List<Books>>> Get()
        {
            var books = _bookService.GetAllBooks();

            if (books == null || books.Count == 0)
                return NotFound(new ApiResponse<List<Books>>
                {
                    Status = 404,
                    Message = "No books available.",
                    Data = null
                });

            return Ok(new ApiResponse<List<Books>>
            {
                Status = 200,
                Message = $"{books.Count} books retrieved successfully.",
                Data = books
            });
        }

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        [SwaggerOperation(Summary = "Get book by ID")]
        [Authorize(Roles = "Admin,Moderator,ReadOnly")]
        public ActionResult<ApiResponse<Books>> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ApiResponse<Books>
                {
                    Status = 400,
                    Message = "Book ID cannot be null or empty.",
                    Data = null
                });

            var book = _bookService.GetBookById(id);
            if (book == null)
                return NotFound(new ApiResponse<Books>
                {
                    Status = 404,
                    Message = "Book not found.",
                    Data = null
                });

            return Ok(new ApiResponse<Books>
            {
                Status = 200,
                Message = "Book retrieved successfully.",
                Data = book
            });
        }

        // ---------------- CREATE ----------------
        [HttpPost("add-one", Name = "AddOneBook")]
        [SwaggerOperation(Summary = "Add one book")]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult<ApiResponse<Books>> Create([FromBody] Books book)
        {
            if (book == null || !ModelState.IsValid)
                return BadRequest(new ApiResponse<Books>
                {
                    Status = 400,
                    Message = "Invalid book data.",
                    Data = null
                });

            _bookService.AddBook(book);

            return CreatedAtRoute("GetBook", new { id = book.Id }, new ApiResponse<Books>
            {
                Status = 201,
                Message = "Book created successfully.",
                Data = book
            });
        }

        [HttpPost("bulk-add", Name = "AddBooksInBulk")]
        [SwaggerOperation(Summary = "Add multiple books in bulk")]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult<ApiResponse<List<Books>>> CreateBulk([FromBody] List<Books> books)
        {
            if (books == null || books.Count == 0 || !ModelState.IsValid)
                return BadRequest(new ApiResponse<List<Books>>
                {
                    Status = 400,
                    Message = "Invalid bulk book data.",
                    Data = null
                });

            _bookService.AddBooksBulk(books);

            return Created("", new ApiResponse<List<Books>>
            {
                Status = 201,
                Message = $"{books.Count} books added successfully.",
                Data = books
            });
        }

        // ---------------- UPDATE ----------------
        [HttpPut("{id:length(24)}", Name = "UpdateBookById")]
        [SwaggerOperation(Summary = "Update book by ID (full replace)")]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult<ApiResponse<object>> Update(string id, [FromBody] Books book)
        {
            if (string.IsNullOrWhiteSpace(id) || book == null || !ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    Message = "Invalid update request.",
                    Data = null
                });

            try
            {
                _bookService.UpdateBook(id, book);
                return Ok(new ApiResponse<object>
                {
                    Status = 200,
                    Message = "Book updated successfully.",
                    Data = null
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    Message = "Book not found.",
                    Data = null
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPatch("{id:length(24)}", Name = "PatchBookById")]
        [SwaggerOperation(Summary = "Partially update book by ID")]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult<ApiResponse<object>> Patch(string id, [FromBody] BookPatchDto patchDto)
        {
            if (string.IsNullOrWhiteSpace(id) || patchDto == null)
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    Message = "Invalid patch request.",
                    Data = null
                });

            try
            {
                _bookService.PatchBook(id, patchDto);
                return Ok(new ApiResponse<object>
                {
                    Status = 200,
                    Message = "Book patched successfully.",
                    Data = null
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    Message = "Book not found.",
                    Data = null
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ---------------- DELETE ----------------
        [HttpDelete("{id:length(24)}", Name = "DeleteBookById")]
        [SwaggerOperation(Summary = "Delete book by ID")]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<object>> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    Message = "Book ID cannot be null or empty.",
                    Data = null
                });

            try
            {
                _bookService.DeleteBook(id);
                return Ok(new ApiResponse<object>
                {
                    Status = 200,
                    Message = "Book deleted successfully.",
                    Data = null
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    Message = "Book not found.",
                    Data = null
                });
            }
        }
    }
}
