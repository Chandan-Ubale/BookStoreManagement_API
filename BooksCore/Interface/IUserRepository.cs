using Books_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Core.Interface
{
    public interface IUserRepository
    {
        User? GetById(string id);
        User? GetByUsername(string username);
        IEnumerable<User> GetAll();
        IEnumerable<User> GetUnverifiedUsers();
        void Create(User user);
        void Update(User user);
        void UpdateRoles(string id, IEnumerable<string> roles);
        void SetVerified(string id, bool isVerified);
        void Delete(string id);
    }
}
