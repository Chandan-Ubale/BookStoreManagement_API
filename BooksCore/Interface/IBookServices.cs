using Books_Core.Models;
using Books_Core.Dtos;
using System.Collections.Generic;

namespace Books_Core.Interface
{
    public interface IBookServices
    {
        List<Books> GetAllBooks();
        Books GetBookById(string id);
        void AddBook(Books book);
        void AddBooksBulk(List<Books> books);
        void UpdateBook(string id, Books book);
        void PatchBook(string id, BookPatchDto patchDto); // ✅ use DTO instead of Books
        void DeleteBook(string id);
    }
}
