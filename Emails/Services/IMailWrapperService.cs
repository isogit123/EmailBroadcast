using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public interface IMailWrapperService
    {
        Task<string> SendMail(string[] to, string displayName, string subject, string body, List<IFormFile> attachments);
    }
}
