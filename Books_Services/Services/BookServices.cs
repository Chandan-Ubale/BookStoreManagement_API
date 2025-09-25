using Books_Core.Dtos;
using Books_Core.Interface;
using Books_Core.Models;
using Microsoft.Extensions.Logging;

namespace Books_Services.Services
{
    public class BookService : IBookServices
    {
        private readonly IBookRepository _repo;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository repo, ILogger<BookService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<Books> GetAllBooks() => _repo.GetAllBooks();

        public Books? GetBookById(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            return _repo.GetBookById(id); // return null if not found
        }

        public void AddBook(Books book)
        {
            ArgumentNullException.ThrowIfNull(book);
            _repo.AddBook(book);
        }

        public void AddBooksBulk(List<Books> books)
        {
            ArgumentNullException.ThrowIfNull(books);
            if (books.Count == 0) throw new ArgumentException("Book list cannot be empty.");
            _repo.AddBooksBulk(books);
        }

        public void UpdateBook(string id, Books book)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(book);

            var existingBook = _repo.GetBookById(id);
            if (existingBook == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }
            _repo.UpdateBook(id, book);
        }

        public void PatchBook(string id, BookPatchDto patchDto)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(patchDto);

            var book = _repo.GetBookById(id);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            if (!string.IsNullOrWhiteSpace(patchDto.Title))
                book.Title = patchDto.Title;

            if (!string.IsNullOrWhiteSpace(patchDto.Author))
                book.Author = patchDto.Author;

            if (patchDto.Price.HasValue)
                book.Price = patchDto.Price.Value;

            _repo.UpdateBook(id, book);
        }

        public void DeleteBook(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            var book = _repo.GetBookById(id);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }
            _repo.DeleteBook(id);
        }
    }

}
