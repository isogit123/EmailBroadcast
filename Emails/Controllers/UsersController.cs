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
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Emails.Filters;

namespace Emails.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUsersService _usersService;
        IMailWrapperService _mailWrapperService;
        public UsersController(IUsersService usersService, IMailWrapperService mailWrapperService)
        {
            _usersService = usersService;
            _mailWrapperService = mailWrapperService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Add([FromBody] Users user)
        {
            if (ModelState.IsValid)
            {
               var errors = await _usersService.AddUser(user);
                if (errors.Count > 0)
                {
                    Response.StatusCode = 400;
                    return string.Join(",", errors);
                }
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

                    Users user = await _usersService.Login(loginViewModel.UserName, loginViewModel.Password);
                    
                    if (user != null)
                    {
                        HttpContext.Response.Cookies.Append("CookieGUID", user.CookieGUID);
                        var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.Id),
};
                        var claimsIdentity = new ClaimsIdentity(
    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            // Refreshing the authentication session should be allowed.
                            
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                            // The time at which the authentication ticket expires. A 
                            // value set here overrides the ExpireTimeSpan option of 
                            // CookieAuthenticationOptions set with AddCookie.

                            //IsPersistent = true,
                            // Whether the authentication session is persisted across 
                            // multiple requests. When used with cookies, controls
                            // whether the cookie's lifetime is absolute (matching the
                            // lifetime of the authentication ticket) or session-based.

                            //IssuedUtc = <DateTimeOffset>,
                            // The time at which the authentication ticket was issued.

                            //RedirectUri = <string>
                            // The full path or absolute URI to be used as an http 
                            // redirect response value.
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);
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
        public async Task LogOut()
        {
            await HttpContext.SignOutAsync(
    CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("CookieGUID");
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

        public async Task<string> ValidatePasswordToken([FromQuery] string token)
        {
            return await _usersService.ValidateToken(token);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task EditPassword([FromBody] ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                string userId = await _usersService.EditUserPassword(resetPasswordViewModel.Token, resetPasswordViewModel.NewPassword);
                await _usersService.ChangeCookieGUID(userId);
                await LogOut();
            }
        }
        [Authorize]
        [ServiceFilter(typeof(CookieAuthorizationFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> EditEmail([FromBody] EditEmailViewModel editEmailViewModel)
        {
            if (ModelState.IsValid)
            {
                string userId = HttpContext.User.Identity.Name;
                var error = await _usersService.EditUserEmail(userId, editEmailViewModel.NewEmail);
                if (!string.IsNullOrEmpty(error))
                {
                    Response.StatusCode = 400;
                    return error;
                }
                await SendConfirmationEmail(userId, editEmailViewModel.NewEmail);
            }
            return "";
        }
        public async Task<ActionResult> ConfirmEmail([FromQuery] string token)
        {
            string userId = await _usersService.ValidateToken(token);
            if (userId != "")
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
                string userId = User.Identity.Name;
                string username = (await _usersService.GetUserById(userId)).Name;
                return username;
            }
            catch
            {
                return "";
            }
        }
        [Authorize]
        public async Task SignOutFromOtherDevices()
        {
            string userId = HttpContext.User.Identity.Name;
            string guid = await _usersService.ChangeCookieGUID(userId);
            HttpContext.Response.Cookies.Append("CookieGUID", guid);
        }
        [Authorize]
        [ServiceFilter(typeof(CookieAuthorizationFilter))]
        public void CheckSession()
        {

        }

        //Sends email confirmation
        private async Task<string> SendConfirmationEmail(string userId, string email)
        {
            string emailConfirmationtoken = await _usersService.GenerateToken(userId);
            var subject = "Email Confirmation";
            var htmlContent = @$"
<html><head></head><body>
<h1>Please confirm your email</h1>
<div>
<a target='_blank' href='{$"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/users/confirmemail/?token={emailConfirmationtoken}"}'>Confirm email</a>
</div>
</body>
</html>
";
            var response = await _mailWrapperService.SendMail(new string[] { email }, "Email Broadcast", subject, htmlContent, null);
            return response != "-1" ? "acc" : "-1";
        }
        private async Task<string> SendResetPasswordEmail(string userId, string email)
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
