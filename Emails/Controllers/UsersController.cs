using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Emails.Services;
using Microsoft.AspNetCore.Mvc;
using Emails.Models;
using Emails.Pages;
using Emails.ViewModels;
using Emails.Filters;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Apache.Ignite.Core;

namespace Emails.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUsersService _usersService;
        IMailWrapperService _mailWrapperService;
        IIgnite _ignite;
        public UsersController(IUsersService usersService, IMailWrapperService mailWrapperService, IIgnite ignite)
        {
            _usersService = usersService;
            _mailWrapperService = mailWrapperService;
            _ignite = ignite;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Add([FromBody] Users user)
        {
            if (ModelState.IsValid)
            {
                await _usersService.AddUser(user);
                if (!string.IsNullOrEmpty(user.Email)) //Send confirmation email.
                    return await SendConfirmationEmail(user.Id, user.Email);
                return "";
            }
            return "";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<int> Login([FromBody] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int userId = await _usersService.Login(loginViewModel.UserName, loginViewModel.Password);
                    if (userId != -1)
                    {
                        HttpContext.Session.SetInt32("userId", userId);
                        //Ignite cache to implement sigout from all devices functionality
                        var cache = _ignite.GetOrCreateCache<int, SessionStore>("sessionCache");
                        cache.Query(new Apache.Ignite.Core.Cache.Query.SqlFieldsQuery($"insert into SessionStore (SessionId,UserId) values ('{HttpContext.Session.Id}', {userId})"));
                        return 0;
                    }
                    return -1;
                }
                catch
                {
                    return -1;
                }
            }
            return -1;
        }
        public void LogOut()
        {
            var cache = _ignite.GetOrCreateCache<int, SessionStore>("sessionCache");
            cache.Query(new Apache.Ignite.Core.Cache.Query.SqlFieldsQuery($"delete from SessionStore where SessionId='{HttpContext.Session.Id}'"));
            HttpContext.Session.Clear();
        }

        public async Task<string> GeneratePasswordToken([FromQuery] string userEmail)
        {
            Users user = await _usersService.GetUserByEmail(userEmail);
            if (user != null)
            {
                if (user.IsEmailConfirmed)
                    return await SendResetPasswordEmail(user.Id, userEmail);
                else
                {
                    await SendConfirmationEmail(user.Id, userEmail);
                    return "-2";
                }
            }
            else
                return "-1"; //No user found.
        }

        public async Task<int> ValidatePasswordToken([FromQuery] string token)
        {
            return await _usersService.ValidateToken(token);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task EditPassword([FromBody] ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                await _usersService.EditUserPassword(resetPasswordViewModel.Token, resetPasswordViewModel.NewPassword);
            }
        }
        [ServiceFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> EditEmail([FromBody] EditEmailViewModel editEmailViewModel)
        {
            if (ModelState.IsValid)
            {
                int? userId = HttpContext.Session.GetInt32("userId");
                if (userId.HasValue)
                {
                    await _usersService.EditUserEmail(userId.Value, editEmailViewModel.NewEmail);
                    return await SendConfirmationEmail(userId.Value, editEmailViewModel.NewEmail);
                }
            }
            return "";
        }
        public async Task<ActionResult> ConfirmEmail([FromQuery] string token)
        {
            int userId = await _usersService.ValidateToken(token);
            if (userId != -1)
            {
                await _usersService.ConfirmUserEmail(userId);
                return Redirect("/login/?v=1");
            }
            return new EmptyResult();
        }
        public async Task<string> GetLoggedInUsername()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("userId");
                string username = (await _usersService.GetUserById(userId.Value)).Name;
                return username;
            }
            catch
            {
                return "";
            }
        }
        [ServiceFilter(typeof(AuthenticationFilter))]
        public void SignOutFromAllDevices()
        {
            var cache = _ignite.GetOrCreateCache<int, SessionStore>("sessionCache");
            cache.Query(new Apache.Ignite.Core.Cache.Query.SqlFieldsQuery($"delete from SessionStore where UserId={HttpContext.Session.GetInt32("userId").Value} and SessionId<>'{HttpContext.Session.Id}'"));

        }
        [ServiceFilter(typeof(AuthenticationFilter))]
        public void CheckSession()
        {

        }

        //Sends email confirmation
        private async Task<string> SendConfirmationEmail(int userId, string email)
        {
            string emailConfirmationtoken = await _usersService.GenerateToken(userId);
            var subject = "Email Confirmation";
            var htmlContent = @$"
<html><head></head><body>
<h1>Please confirm your email</h1>
<div>
<a target='_blank' href='{ $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/users/confirmemail/?token={emailConfirmationtoken}"}'>Confirm email</a>
</div>
</body>
</html>
";
            var response = await _mailWrapperService.SendMail(new string[] { email }, "Email Broadcast", subject, htmlContent, null);
            return response != "-1" ? "acc" : "-1";
        }
        private async Task<string> SendResetPasswordEmail(int userId, string email)
        {
            string resetPassworToken = await _usersService.GenerateToken(userId);
            var subject = "Reset Password";
            var htmlContent = @$"
<html><head></head><body>
<h1>Please click the link below to reset your password</h1>
<div>
<a target='_blank' href='{$"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/resetpassword/?token={resetPassworToken}"}'>Reset password</a>
</div>
</body>
</html>
";
            var response = await _mailWrapperService.SendMail(new string[] { email }, "Email Broadcast", subject, htmlContent, null);
            return response != "-1" ? "acc" : "-1";
        }

    }
}
