using Apache.Ignite.Core;
using Emails.Services;
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
    public class AuthenticationFilter : Attribute, IAuthorizationFilter
    {
        IIgnite _ignite;
        public AuthenticationFilter(IIgnite ignite)
        {
            _ignite = ignite;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            int? userId = context.HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
                context.Result = new UnauthorizedResult();
            else
            {
                //Clear session if user requested sign out from all devices.
                var cache = _ignite.GetOrCreateCache<int, SessionStore>("sessionCache");
                var cursor = cache.Query(new Apache.Ignite.Core.Cache.Query.SqlFieldsQuery($"select * from SessionStore where SessionId='{context.HttpContext.Session.Id}'"));
                bool exists = cursor.GetEnumerator().MoveNext();
                if (!exists)
                {
                    context.HttpContext.Session.Clear();
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
