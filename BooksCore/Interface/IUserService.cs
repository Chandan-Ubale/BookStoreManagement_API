using Books_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books_Core.Interface
{
    public interface IUserService
    {
        User Register(string username, string email, string password);
        User? GetByUsername(string username);
        IEnumerable<User> GetUnverifiedUsers();
        void VerifyUser(string id);
        void UpdateRoles(string id, IEnumerable<string> roles);
        IEnumerable<User> GetAllUsers();
    }
}
