using Xunit;
using Mongo2Go;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Books_Infrastructure.Data;      
using Books_Infrastructure.Repositories; 
using Books_Core.Models;

namespace BooksInfrastructure.Tests
{
    public class BookRepositoryUnitTest : IDisposable
    {
        private readonly MongoDbRunner _runner;
        private readonly BookRepository _repo;

        public BookRepositoryUnitTest()
        {
            // start embedded MongoDB
            _runner = MongoDbRunner.Start();

            // wire up options for MongoDbContext
            var options = Options.Create(new Books_Core.Models.BookstoreDatabaseSettings
            {
                ConnectionString = _runner.ConnectionString,
                DatabaseName = "TestDb",
                BooksCollectionName = "Books"
            });

            var context = new MongoDbContext(options);
            _repo = new BookRepository(context);
        }

        [Fact]
        public void AddAndGetBook_Works()
        {
            var book = new Books { Title = "T", Author = "A", Price = 99.9M };
            _repo.AddBook(book);

            var all = _repo.GetAllBooks();
            Assert.Contains(all, b => b.Title == "T" && b.Author == "A");
        }
        [Fact]
        public void UpdateBook_ThrowsKeyNotFound_WhenMissing()
        {
            var missing = new Books { Id = "507f1f77bcf86cd799439011", Title = "X", Author = "Y", Price = 10 };
            Assert.Throws<KeyNotFoundException>(() => _repo.UpdateBook(missing.Id!, missing));
        }

        [Fact]
        public void DeleteBook_ThrowsKeyNotFound_WhenMissing()
        {
            Assert.Throws<KeyNotFoundException>(() => _repo.DeleteBook("507f1f77bcf86cd799439011"));
        }

        [Fact]
        public void UpdateBook_Works()
        {
            var book = new Books { Title = "Old", Author = "A", Price = 10M };
            _repo.AddBook(book);

            book.Title = "New";
            _repo.UpdateBook(book.Id!, book);

            var fetched = _repo.GetBookById(book.Id!);
            Assert.Equal("New", fetched!.Title);
        }

        [Fact]
        public void DeleteBook_RemovesBook()
        {
            var book = new Books { Title = "Temp", Author = "A", Price = 10M };
            _repo.AddBook(book);

            _repo.DeleteBook(book.Id!);

            var fetched = _repo.GetBookById(book.Id!);
            Assert.Null(fetched);
        }

        [Fact]
        public void GetBookById_ReturnsNullWhenMissing()
        {
            var result = _repo.GetBookById("507f1f77bcf86cd799439011"); // random id
            Assert.Null(result);
        }

        public void Dispose()
        {
            _runner.Dispose();
        }
    }
}