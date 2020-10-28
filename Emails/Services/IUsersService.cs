using Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public interface IUsersService
    {
        Task<Users> GetUserById(int userId);
        Task AddUser(Users user);
        Task EditUserPassword(string token, string newPassword);
        Task EditUserEmail(int userId, string newEmail);
        Task<int> Login(string username, string password);
        Task<string> GenerateToken(int userId);
        Task<Users> GetUserByEmail(string email);
        Task<int> ValidateToken(string token);
        Task ConfirmUserEmail(int userId);
    }
}
