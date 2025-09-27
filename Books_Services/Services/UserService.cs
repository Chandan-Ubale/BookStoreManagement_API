using Books_Core.Interface;
using Books_Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository repo, ILogger<UserService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public User Register(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.");

            var existing = _repo.GetByUsername(username);
            if (existing != null)
                throw new InvalidOperationException("Username already taken.");

            var hashed = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = hashed,
                IsVerified = false,
                Roles = new List<string> { "ReadOnly" } // default role
            };

            _repo.Create(user);
            _logger.LogInformation("New user registered: {username}", username);
            return user;
        }

        public User? GetByUsername(string username) => _repo.GetByUsername(username);

        public IEnumerable<User> GetUnverifiedUsers() => _repo.GetUnverifiedUsers();

        public void VerifyUser(string id)
        {
            var u = _repo.GetById(id);
            if (u == null) throw new KeyNotFoundException("User not found.");
            _repo.SetVerified(id, true);
            _logger.LogInformation("User verified: {id}", id);
        }

        public void UpdateRoles(string id, IEnumerable<string> roles)
        {
            var u = _repo.GetById(id);
            if (u == null) throw new KeyNotFoundException("User not found.");
            // Basic validation: at least one role
            var newRoles = roles?.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).Distinct().ToList()
                ?? new List<string>();
            if (newRoles.Count == 0) throw new ArgumentException("At least one role must be provided.");
            _repo.UpdateRoles(id, newRoles);
            _logger.LogInformation("Updated roles for user {id}: {roles}", id, string.Join(",", newRoles));
        }

        public IEnumerable<User> GetAllUsers() => _repo.GetAll();
    }
}
