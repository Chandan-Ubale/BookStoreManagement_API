using Books_Core.Interface;
using Books_Core.Models;
using System;
using System.Collections.Generic;

namespace Books_Services.Services
{
    public class BookService : IBookServices
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public List<Books> GetAllBooks() => _repository.GetAllBooks();

        public Books? GetBookById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id), "Book ID cannot be null or empty.");

            return _repository.GetBookById(id);
        }

        public void AddBook(Books book)
        {
            ValidateBook(book);
            _repository.AddBook(book);
        }

        public void AddBooksBulk(List<Books> books)
        {
            if (books == null)
                throw new ArgumentNullException(nameof(books), "Books list cannot be null.");
            if (books.Count == 0)
                throw new ArgumentException("Books list cannot be empty.", nameof(books));

            foreach (var book in books)
            {
                ValidateBook(book);
            }

            _repository.AddBooksBulk(books);
        }

        public void UpdateBook(string id, Books bookIn)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id), "Book ID cannot be null or empty.");
            ValidateBook(bookIn);

            _repository.UpdateBook(id, bookIn);
        }

        public void DeleteBook(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id), "Book ID cannot be null or empty.");

            _repository.DeleteBook(id);
        }

        // Private helper for validating individual Books
        private void ValidateBook(Books book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book), "Book cannot be null.");
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title cannot be empty.", nameof(book.Title));
            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Book author cannot be empty.", nameof(book.Author));
            if (book.Price <= 0)
                throw new ArgumentException("Book price must be greater than 0.", nameof(book.Price));
        }
    }
}
