using Books_Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Books_Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public IMongoCollection<Books> BooksCollection { get; }
        public IMongoCollection<User> UsersCollection { get; }

        public MongoDbContext(IOptions<BookstoreDatabaseSettings> options)
        {
            var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);

            BooksCollection = _database.GetCollection<Books>(settings.BooksCollectionName);
            // Users collection name set to "Users"
            UsersCollection = _database.GetCollection<User>("Users");
        }
    }
}
