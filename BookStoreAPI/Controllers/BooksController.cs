using Books_Core.Interface;
using Books_Core.Models;
using Books_Services.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookStoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookServices _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookServices bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all books")]
        public ActionResult<List<Books>> Get()
        {
            _logger.LogInformation("Fetching all books.");

            try
            {
                var books = _bookService.GetAllBooks();

                if (books == null || books.Count == 0)
                {
                    _logger.LogWarning("No books found in the database.");
                    return NotFound("No books available.");
                }

                _logger.LogInformation("Successfully fetched {Count} books.", books.Count);
                return books;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        [SwaggerOperation(Summary = "Get book by ID")]
        public ActionResult<Books> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Get book called with null or empty ID.");
                return BadRequest("Book ID cannot be null or empty.");
            }

            _logger.LogInformation("Fetching book with ID: {BookId}", id);

            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully fetched book with ID: {BookId}", id);
            return book;
        }

        [HttpPost("add-one", Name = "AddOneBook")]
        [SwaggerOperation(Summary = "Add one book")]
        public ActionResult<Books> Create(Books book)
        {
            if (book == null)
            {
                _logger.LogWarning("Create called with null book.");
                return BadRequest("Book cannot be null.");
            }

            _logger.LogInformation("Adding a new book: {Title}", book.Title);

            _bookService.AddBook(book);

            _logger.LogInformation("Book with ID {BookId} added successfully.", book.Id);
            return CreatedAtRoute("GetBook", new { id = book.Id }, book);
        }

        [HttpPost("bulk-add", Name = "AddBooksInBulk")]
        [SwaggerOperation(Summary = "Add multiple books in bulk")]
        public ActionResult<List<Books>> CreateBulk(List<Books> books)
        {
            if (books == null || books.Count == 0)
            {
                _logger.LogWarning("CreateBulk called with null or empty list.");
                return BadRequest("Book list cannot be null or empty.");
            }

            _logger.LogInformation("Adding {Count} books in bulk.", books.Count);

            _bookService.AddBooksBulk(books);

            _logger.LogInformation("{Count} books added successfully in bulk.", books.Count);
            return Created("", books);
        }

        [HttpPut("{id:length(24)}", Name = "UpdateBookById")]
        [SwaggerOperation(Summary = "Update book by ID")]
        public IActionResult Update(string id, Books bookIn)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Update called with null or empty ID.");
                return BadRequest("Book ID cannot be null or empty.");
            }

            if (bookIn == null)
            {
                _logger.LogWarning("Update called with null book.");
                return BadRequest("Book cannot be null.");
            }

            _logger.LogInformation("Updating book with ID: {BookId}", id);

            try
            {
                _bookService.UpdateBook(id, bookIn);
                _logger.LogInformation("Book with ID {BookId} updated successfully.", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Book with ID {BookId} not found for update.", id);
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data provided for updating book with ID {BookId}.", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating book with ID {BookId}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteBookById")]
        [SwaggerOperation(Summary = "Delete book by ID")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Delete called with null or empty ID.");
                return BadRequest("Book ID cannot be null or empty.");
            }

            _logger.LogInformation("Deleting book with ID: {BookId}", id);

            try
            {
                _bookService.DeleteBook(id);
                _logger.LogInformation("Book with ID {BookId} deleted successfully.", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Book with ID {BookId} not found for deletion.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting book with ID {BookId}.", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
