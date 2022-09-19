using Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public interface IUsersService
    {
        Task<Users> GetUserById(string userId);
        Task<List<string>> AddUser(Users user);
        Task<List<string>> UpdateUser(Users user);
        Task<string> EditUserPassword(string token, string newPassword);
        Task<string> EditUserEmail(string userId, string newEmail);
        Task<Users> Login(string username, string password);
        Task<string> GenerateToken(string userId);
        Task<Users> GetUserByEmail(string email);
        Task<string> ValidateToken(string token);
        Task ConfirmUserEmail(string userId);
        Task<List<string>> ValidateUser(Users user);
        Task<string> ChangeCookieGUID(string userId);
        Task<Users> GetUserByName(string userName);
    }
}
