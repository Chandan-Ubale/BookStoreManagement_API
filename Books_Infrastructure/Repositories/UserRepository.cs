using Books_Core.Interface;
using Books_Core.Models;
using Books_Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context)
        {
            // MongoDbContext must expose UsersCollection (see update below).
            _users = context.UsersCollection;
        }

        public User? GetById(string id) =>
            _users.Find(u => u.Id == id).FirstOrDefault();

        public User? GetByUsername(string username) =>
            _users.Find(u => u.UserName == username).FirstOrDefault();

        public IEnumerable<User> GetAll() =>
            _users.Find(_ => true).ToList();

        public IEnumerable<User> GetUnverifiedUsers() =>
            _users.Find(u => u.IsVerified == false).ToList();

        public void Create(User user) =>
            _users.InsertOne(user);

        public void Update(User user) =>
            _users.ReplaceOne(u => u.Id == user.Id, user);

        public void UpdateRoles(string id, IEnumerable<string> roles) =>
            _users.UpdateOne(u => u.Id == id,
                Builders<User>.Update.Set(x => x.Roles, roles.ToList()));

        public void SetVerified(string id, bool isVerified) =>
            _users.UpdateOne(u => u.Id == id,
                Builders<User>.Update.Set(x => x.IsVerified, isVerified));

        public void Delete(string id) =>
            _users.DeleteOne(u => u.Id == id);
    }
}
