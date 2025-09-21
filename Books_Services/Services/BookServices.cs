using Books_Core.Interface;
using Books_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Services.Services
{
    public class BookService : IBookServices
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }

        public List<Books> GetAllBooks() => _repository.GetAllBooks();
        public Books? GetBookById(string id) => _repository.GetBookById(id);
        public void AddBook(Books book) => _repository.AddBook(book);
        public void AddBooksBulk(List<Books> books) => _repository.AddBooksBulk(books);
        public void UpdateBook(string id, Books bookIn) => _repository.UpdateBook(id, bookIn);
        public void DeleteBook(string id) => _repository.DeleteBook(id);
    }
}
