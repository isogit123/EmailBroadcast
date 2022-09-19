using Emails.Models;
using Emails.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CookieAuthorizationFilter : Attribute, IAsyncActionFilter
    {
        private readonly IUsersService _usersService;
        public CookieAuthorizationFilter(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (string.IsNullOrEmpty(context.HttpContext.Request.Cookies["CookieGUID"]))
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                string userId = context.HttpContext.User.Identity.Name;
                Users users = await _usersService.GetUserById(userId);

                if (users.CookieGUID != context.HttpContext.Request.Cookies["CookieGUID"])
                {
                    await context.HttpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);
                    context.HttpContext.Response.Cookies.Delete("CookieGUID");
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    await next();
                }
            }
        }
    }
}
