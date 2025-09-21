using Books_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Core.Interface
{
    public interface IBookRepository
    {
        List<Books> GetAllBooks();
        Books? GetBookById(string id);
        void AddBook(Books book);
        void AddBooksBulk(List<Books> books);
        void UpdateBook(string id, Books bookIn);
        void DeleteBook(string id);
    }
}
