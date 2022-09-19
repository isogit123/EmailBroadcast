using Emails.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.WebUtilities;
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
        private FirestoreDb _db;

        public UsersService(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<Users> GetUserById(string userId)
        {
            DocumentReference documentReference = _db.Collection("users").Document(userId);
            var snapshot = await documentReference.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Users user = snapshot.ConvertTo<Users>();
                return user;
            }
            else
                return null;
        }       
        public async Task<Users> GetUserByName(string userName)
        {
            Query query = _db.Collection("users").WhereEqualTo("Name", userName.ToLower());
            var querySnapshot = await query.GetSnapshotAsync();
            if (querySnapshot.Count > 0)
            {
                Users user = querySnapshot.First().ConvertTo<Users>();
                return user;
            }
            else
                return null;
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            Query query = _db.Collection("users").WhereEqualTo("Email", email.ToLower());
            var snapshot = (await query.GetSnapshotAsync()).First();
            if (snapshot.Exists)
            {
                Users user = snapshot.ConvertTo<Users>();
                user.Id = snapshot.Id;
                return user;
            }
            else
                return null;
        }
        private async Task<string> ValidateUserNameUnique(string name)
        {
            Query query = _db.Collection("users").WhereEqualTo("Name", name);
            var isExist = (await query.GetSnapshotAsync()).Count > 0;
            if (isExist)
                return "User name already exists";
            return "";
        }
        private async Task<string> ValidateEmailUnique(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.ToLower();
                Query query = _db.Collection("users").WhereEqualTo("Email", email);
                var emailExists = (await query.GetSnapshotAsync()).Count > 0;
                if (emailExists)
                    return "Email already exists";
            }
            return "";
        }
        public async Task<List<string>> ValidateUser(Users user)
        {
            List<string> errors = new List<string>();
            string userNameUniqueError = await ValidateUserNameUnique(user.Name);
            string emailUniqueError = await ValidateEmailUnique(user.Email);
            if (!string.IsNullOrEmpty(userNameUniqueError))
                errors.Add(emailUniqueError);
            if (!string.IsNullOrEmpty(emailUniqueError))
                errors.Add(emailUniqueError);
            return errors;
        }
        public async Task<List<string>> AddUser(Users user)
        {
            var errors = await ValidateUser(user);
            if (errors.Count == 0)
            {
                user.Name = user.Name.ToLower();
                user.Password = Hash(user.Password);
                user.CookieGUID = Guid.NewGuid().ToString();
                var res = await _db.Collection("users").AddAsync(user);
                user.Id = res.Id;
            }
            return errors;
        }
        public async Task<List<string>> UpdateUser(Users user)
        {
            var errors = await ValidateUser(user);
            if (errors.Count == 0)
            {
                if (!string.IsNullOrEmpty(user.Email))
                    user.Email = user.Email.ToLower();
                await _db.Collection("users").Document(user.Id).SetAsync(user);
            }
            return errors;
        }
        public async Task<string> EditUserPassword(string token, string newPassword)
        {
            string userId = await ValidateToken(WebUtility.UrlDecode(token));
            string pass = Hash(newPassword);
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates["Password"] = pass;
            DocumentReference documentReference = _db.Collection("users").Document(userId);
            await documentReference.UpdateAsync(updates);
            return userId;
        }
        public async Task<string> EditUserEmail(string userId, string newEmail)
        {
            var error = await ValidateEmailUnique(newEmail);
            if (string.IsNullOrEmpty(error))
            {
                Dictionary<string, object> updates = new Dictionary<string, object>();
                updates["Email"] = newEmail;
                updates["IsEmailConfirmed"] = false;
                DocumentReference documentReference = _db.Collection("users").Document(userId);
                await documentReference.UpdateAsync(updates);
            }
            return error;
        }
        private string Hash(string value, int? iterations = null)
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
        private string Hash(string value, string saltToUse, int? iterations = null)
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

        public async Task<Users> Login(string username, string password)
        {
            Users user = await GetUserByName(username);
            if (user != null)
            {
                string[] splitPassword = user.Password.Split(",");
                string hash = Hash(password, splitPassword[1]);
                if (splitPassword[0] == hash)
                    return user;
            }
            return null;
        }

        public async Task<string> GenerateToken(string userId)
        {
            string token = "";
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {

                byte[] bytes = new byte[18]; // Use a multiple of 3 (e.g. 3, 6, 12) to prevent output with trailing padding '=' characters).
                rng.GetBytes(bytes);

                token = WebEncoders.Base64UrlEncode(bytes);
            }
            string tokenHash = Hash(token, 1);
            await _db.Collection("tokens").AddAsync(new Tokens
            {
                UserId = userId,
                GenerationDate = DateTime.UtcNow,
                Token = tokenHash
            });
            string salt = WebUtility.UrlEncode(tokenHash.Split(",")[1]);
            return token + $",{salt}";
        }
        public async Task<string> ValidateToken(string token)
        {
            string[] tokenSplit = token.Split(",");
            string salt = tokenSplit[1];
            string tokenHash = Hash(tokenSplit[0], salt, 1);
            DateTime maxTime = DateTime.UtcNow.AddHours(1);
            Query query = _db.Collection("tokens").WhereEqualTo("Token", tokenHash + $",{salt}")
                .WhereLessThanOrEqualTo("GenerationDate", maxTime);
            var snapshots = await query.GetSnapshotAsync();
            Tokens tokenFetched = snapshots.Documents.FirstOrDefault().ConvertTo<Tokens>();
            if (tokenFetched != null)
                return tokenFetched.UserId;
            else
                return "";
        }
        public async Task ConfirmUserEmail(string userId)
        {
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates["IsEmailConfirmed"] = true;
            DocumentReference documentReference = _db.Collection("users").Document(userId);
            await documentReference.UpdateAsync(updates);
        }
        public async Task<string> ChangeCookieGUID(string userId)
        {
            DocumentReference documentReference = _db.Collection("users").Document(userId);
            string guid = Guid.NewGuid().ToString();
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates["CookieGUID"] = guid;
            await documentReference.UpdateAsync(updates);
            return guid;
        }

    }
}
