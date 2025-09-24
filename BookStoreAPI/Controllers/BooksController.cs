using Books_Core.Interface;
using Books_Core.Models;
using Books_Core.Dtos;
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
            _logger.LogInformation("HTTP GET /books called");
            var books = _bookService.GetAllBooks();
            if (books == null || books.Count == 0)
            {
                _logger.LogInformation("No books found");
                return NotFound("No books available.");
            }
            _logger.LogInformation("{Count} books returned", books.Count);
            return Ok(books);
        }

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        [SwaggerOperation(Summary = "Get book by ID")]
        public ActionResult<Books> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Get by ID called with null or empty Id");
                return BadRequest("Book ID cannot be null or empty.");
            }

            _logger.LogInformation("Fetching book with Id: {Id}", id);
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                _logger.LogWarning("Book not found with Id: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Book retrieved: {Title}", book.Title);
            return Ok(book);
        }

        [HttpPost("add-one", Name = "AddOneBook")]
        [SwaggerOperation(Summary = "Add one book")]
        public ActionResult<Books> Create([FromBody] Books book)
        {
            if (book == null)
            {
                _logger.LogWarning("Attempted to POST null book");
                return BadRequest("Book cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid book model received");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new book: {Title}", book.Title);
            _bookService.AddBook(book);
            _logger.LogInformation("Book created with Id: {Id}", book.Id);
            return CreatedAtRoute("GetBook", new { id = book.Id }, book);
        }

        [HttpPost("bulk-add", Name = "AddBooksInBulk")]
        [SwaggerOperation(Summary = "Add multiple books in bulk")]
        public ActionResult<List<Books>> CreateBulk([FromBody] List<Books> books)
        {
            if (books == null || books.Count == 0)
            {
                _logger.LogWarning("Bulk add called with null or empty list");
                return BadRequest("Book list cannot be null or empty.");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid bulk book model received");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding {Count} books in bulk", books.Count);
            _bookService.AddBooksBulk(books);
            _logger.LogInformation("Bulk add completed");
            return Created("", books);
        }

        [HttpPut("{id:length(24)}", Name = "UpdateBookById")]
        [SwaggerOperation(Summary = "Update book by ID (full replace)")]
        public IActionResult Update(string id, [FromBody] Books book)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Update called with null or empty Id");
                return BadRequest("Book ID cannot be null or empty.");
            }
            if (book == null)
            {
                _logger.LogWarning("Update called with null book data");
                return BadRequest("Book data cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid book model received for update");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Updating book Id: {Id}", id);
                _bookService.UpdateBook(id, book);
                _logger.LogInformation("Book Id {Id} updated successfully", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Book Id {Id} not found for update", id);
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid update attempt for book Id {Id}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id:length(24)}", Name = "PatchBookById")]
        [SwaggerOperation(Summary = "Partially update book by ID")]
        public IActionResult Patch(string id, [FromBody] BookPatchDto patchDto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Patch called with null or empty Id");
                return BadRequest("Book ID cannot be null or empty.");
            }
            if (patchDto == null)
            {
                _logger.LogWarning("Patch called with null patch data");
                return BadRequest("Patch data cannot be null.");
            }

            try
            {
                _logger.LogInformation("Patching book Id: {Id} with data {@PatchDto}", id, patchDto);
                _bookService.PatchBook(id, patchDto);
                _logger.LogInformation("Book Id {Id} patched successfully", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Book Id {Id} not found for patch", id);
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid patch attempt for book Id {Id}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteBookById")]
        [SwaggerOperation(Summary = "Delete book by ID")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Delete called with null or empty Id");
                return BadRequest("Book ID cannot be null or empty.");
            }

            try
            {
                _logger.LogInformation("Deleting book Id: {Id}", id);
                _bookService.DeleteBook(id);
                _logger.LogInformation("Book Id {Id} deleted successfully", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Book Id {Id} not found for deletion", id);
                return NotFound();
            }
        }
    }
}
