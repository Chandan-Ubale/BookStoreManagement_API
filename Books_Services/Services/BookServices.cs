using Books_Core.Interface;
using Books_Core.Models;
using Books_Core.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Books_Services.Services
{
    public class BookService : IBookServices
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository repository, ILogger<BookService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
        }

        public List<Books> GetAllBooks()
        {
            _logger.LogInformation("Fetching all books from repository");
            var books = _repository.GetAllBooks();
            _logger.LogInformation("Retrieved {Count} books", books.Count);
            return books;
        }

        public Books GetBookById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("GetBookById called with null or empty id");
                throw new ArgumentNullException(nameof(id));
            }

            _logger.LogInformation("Fetching book with Id: {Id}", id);
            var book = _repository.GetBookById(id);
            if (book == null)
                _logger.LogWarning("Book not found with Id: {Id}", id);
            else
                _logger.LogInformation("Book retrieved: {Title}", book.Title);

            return book!;
        }

        public void AddBook(Books book)
        {
            if (book == null)
            {
                _logger.LogWarning("Attempted to add a null book");
                throw new ArgumentNullException(nameof(book));
            }

            _logger.LogInformation("Adding book: {Title} by {Author}", book.Title, book.Author);
            _repository.AddBook(book);
            _logger.LogInformation("Book added with Id: {Id}", book.Id);
        }

        public void AddBooksBulk(List<Books> books)
        {
            if (books == null)
            {
                _logger.LogWarning("Attempted to add null book list");
                throw new ArgumentNullException(nameof(books));
            }
            if (books.Count == 0)
            {
                _logger.LogWarning("Attempted to add empty book list");
                throw new ArgumentException("Book list cannot be empty", nameof(books));
            }

            _logger.LogInformation("Adding {Count} books in bulk", books.Count);
            _repository.AddBooksBulk(books);
            _logger.LogInformation("Bulk add completed");
        }

        public void UpdateBook(string id, Books bookIn)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("UpdateBook called with null or empty Id");
                throw new ArgumentNullException(nameof(id));
            }
            if (bookIn == null)
            {
                _logger.LogWarning("UpdateBook called with null book data");
                throw new ArgumentNullException(nameof(bookIn));
            }

            _logger.LogInformation("Updating book Id: {Id}", id);
            _repository.UpdateBook(id, bookIn);
            _logger.LogInformation("Book Id {Id} updated successfully", id);
        }

        public void PatchBook(string id, BookPatchDto patchDto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("PatchBook called with null or empty Id");
                throw new ArgumentNullException(nameof(id));
            }
            if (patchDto == null)
            {
                _logger.LogWarning("PatchBook called with null patch data");
                throw new ArgumentNullException(nameof(patchDto));
            }

            _logger.LogInformation("Patching book Id: {Id} with data {@PatchDto}", id, patchDto);
            var book = _repository.GetBookById(id);
            if (book == null)
            {
                _logger.LogWarning("Book not found for patch Id: {Id}", id);
                throw new KeyNotFoundException($"Book not found with Id: {id}");
            }

            if (!string.IsNullOrWhiteSpace(patchDto.Title)) book.Title = patchDto.Title;
            if (!string.IsNullOrWhiteSpace(patchDto.Author)) book.Author = patchDto.Author;
            if (patchDto.Price.HasValue) book.Price = patchDto.Price.Value;

            _repository.UpdateBook(id, book);
            _logger.LogInformation("Book Id {Id} patched successfully", id);
        }

        public void DeleteBook(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("DeleteBook called with null or empty Id");
                throw new ArgumentNullException(nameof(id));
            }

            _logger.LogInformation("Deleting book Id: {Id}", id);
            _repository.DeleteBook(id);
            _logger.LogInformation("Book Id {Id} deleted successfully", id);
        }
    }
}
