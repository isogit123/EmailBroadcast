using Emails.DB;
using Emails.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Emails.Services
{
    public class UsersService : IUsersService
    {
        private Context _db;

        public UsersService(Context db)
        {
            _db = db;
        }

        public async Task<Users> GetUserById(int userId)
        {
            return await _db.Users.FindAsync(userId);
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            return await _db.Users.Where(e => e.Email == email.ToLower()).FirstOrDefaultAsync();
        }
        public async Task AddUser(Users user)
        {
            user.Password = Hash(user.Password);
            if (!string.IsNullOrEmpty(user.Email))
                user.Email = user.Email.ToLower();
            await _db.AddAsync(user);
            await _db.SaveChangesAsync();
        }
        public async Task EditUserPassword(string token, string newPassword)
        {
            int userId = await ValidateToken(WebUtility.UrlDecode(token));
            Users user = await _db.Users.FindAsync(userId);
            user.Password = Hash(newPassword);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
        public async Task EditUserEmail(int userId, string newEmail)
        {
            Users user = await _db.Users.FindAsync(userId);
            user.Email = newEmail.ToLower();
            user.IsEmailConfirmed = false;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
        public string Hash(string value, int? iterations = null)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: value,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: !iterations.HasValue ? 10000 : iterations.Value,
                numBytesRequested: 256 / 8));
            return hashed + "," + Convert.ToBase64String(salt);
        }
        public string Hash(string value, string saltToUse, int? iterations = null)
        {
            byte[] salt = Convert.FromBase64String(saltToUse);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: value,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: !iterations.HasValue ? 10000 : iterations.Value,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public async Task<int> Login(string username, string password)
        {
            Users user = await _db.Users.Where(e => e.Name == username).FirstOrDefaultAsync();
            if (user != null)
            {
                string[] splitPassword = user.Password.Split(",");
                string hash = Hash(password, splitPassword[1]);
                if (splitPassword[0] == hash)
                    return user.Id;
            }
            return -1;
        }

        public async Task<string> GenerateToken(int userId)
        {
            string token = "";
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {

                byte[] bytes = new byte[18]; // Use a multiple of 3 (e.g. 3, 6, 12) to prevent output with trailing padding '=' characters).
                rng.GetBytes(bytes);

                token = WebEncoders.Base64UrlEncode(bytes);
            }
            string tokenHash = Hash(token, 1);
            await _db.Tokens.AddAsync(new Tokens
            {
                UserId = userId,
                GenerationDate = DateTime.Now,
                Token = tokenHash
            });
            await _db.SaveChangesAsync();
            string salt = WebUtility.UrlEncode(tokenHash.Split(",")[1]);
            return token + $",{salt}";
        }
        public async Task<int> ValidateToken(string token)
        {
            string[] tokenSplit = token.Split(",");
            string salt = tokenSplit[1];
            string tokenHash = Hash(tokenSplit[0], salt, 1);
            DateTime maxTime = DateTime.Now.AddHours(1);
            Tokens tokenFetched = await _db.Tokens.Where(e => e.Token == tokenHash + $",{salt}" && e.GenerationDate <= maxTime).FirstOrDefaultAsync();
            if (tokenFetched != null)
                return tokenFetched.UserId;
            else
                return -1;
        }
        public async Task ConfirmUserEmail(int userId)
        {
            Users user = await _db.Users.FindAsync(userId);
            user.IsEmailConfirmed = true;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
