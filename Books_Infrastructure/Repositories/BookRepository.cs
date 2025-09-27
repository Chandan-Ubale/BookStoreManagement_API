using Books_Core.Interface;
using Books_Core.Models;
using Books_Core.Dtos;
using Books_Infrastructure.Data;
using MongoDB.Driver;

namespace Books_Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IMongoCollection<Books> _collection;

        public BookRepository(MongoDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _collection = context.BooksCollection; // ✅ FIXED
        }

        // Get all books
        public List<Books> GetAllBooks() => _collection.Find(_ => true).ToList();

        // Get book by Id
        public Books? GetBookById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return _collection.Find(b => b.Id == id).FirstOrDefault();
        }

        // Add a single book
        public void AddBook(Books book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            _collection.InsertOne(book);
        }

        // Add multiple books
        public void AddBooksBulk(List<Books> books)
        {
            if (books == null) throw new ArgumentNullException(nameof(books));
            if (books.Count == 0) throw new ArgumentException("Book list cannot be empty.", nameof(books));

            _collection.InsertMany(books);
        }

        // Update full book (replace document)
        public void UpdateBook(string id, Books bookIn)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (bookIn == null) throw new ArgumentNullException(nameof(bookIn));

            bookIn.Id = id; // ensure immutable _id

            var result = _collection.ReplaceOne(b => b.Id == id, bookIn);

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException($"Book with id '{id}' not found.");
        }

        // Patch book (partial update with DTO)
        public void PatchBook(string id, BookPatchDto patchDto)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (patchDto == null) throw new ArgumentNullException(nameof(patchDto));

            var updates = new List<UpdateDefinition<Books>>();

            if (!string.IsNullOrWhiteSpace(patchDto.Title))
                updates.Add(Builders<Books>.Update.Set(b => b.Title, patchDto.Title));

            if (!string.IsNullOrWhiteSpace(patchDto.Author))
                updates.Add(Builders<Books>.Update.Set(b => b.Author, patchDto.Author));

            if (patchDto.Price.HasValue)
                updates.Add(Builders<Books>.Update.Set(b => b.Price, patchDto.Price.Value));

            if (!updates.Any())
                throw new ArgumentException("No valid fields provided for update.", nameof(patchDto));

            var update = Builders<Books>.Update.Combine(updates);

            var result = _collection.UpdateOne(b => b.Id == id, update);

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException($"Book with id '{id}' not found.");
        }

        // Delete book by Id
        public void DeleteBook(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            var result = _collection.DeleteOne(b => b.Id == id);

            if (result.DeletedCount == 0)
                throw new KeyNotFoundException($"Book with id '{id}' not found.");
        }
    }
}
