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

        public BooksController(IBookServices bookService)  
        {
            _bookService = bookService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all books")]
        public ActionResult<List<Books>> Get() => _bookService.GetAllBooks();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        [SwaggerOperation(Summary = "Get book by ID")]
        public ActionResult<Books> Get(string id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null) return NotFound();
            return book;
        }

        [HttpPost("add-one", Name = "AddOneBook")]
        [SwaggerOperation(Summary = "Add one book")]
        public ActionResult<Books> Create(Books book)
        {
            _bookService.AddBook(book);
            return CreatedAtRoute("GetBook", new { id = book.Id }, book);
        }

        [HttpPost("bulk-add", Name = "AddBooksInBulk")]
        [SwaggerOperation(Summary = "Add multiple books in bulk")]
        public ActionResult<List<Books>> CreateBulk(List<Books> books)
        {
            _bookService.AddBooksBulk(books);
            return Created("", books);
        }

        [HttpPut("{id:length(24)}", Name = "UpdateBookById")]
        [SwaggerOperation(Summary = "Update book by ID")]
        public IActionResult Update(string id, Books bookIn)
        {
            try
            {
                _bookService.UpdateBook(id, bookIn);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteBookById")]
        [SwaggerOperation(Summary = "Delete book by ID")]
        public IActionResult Delete(string id)
        {
            try
            {
                _bookService.DeleteBook(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
