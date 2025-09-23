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