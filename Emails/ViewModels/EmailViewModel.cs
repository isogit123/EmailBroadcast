using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.ViewModels
{
    public class EmailViewModel
    {
        public string GroupId { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
        public List<IFormFile> Attachments{ get; set; }
    }
}
