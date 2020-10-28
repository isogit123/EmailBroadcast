using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emails.Filters;
using Emails.Models;
using Emails.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Emails.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(AuthenticationFilter))]
    public class SentEmailsController : ControllerBase
    {
        ISentEmailsService _sentEmailsService;
        public SentEmailsController(ISentEmailsService sentEmailsService)
        {
            _sentEmailsService = sentEmailsService;
        }

        public async Task<List<SentEmails>> GetEmails([FromQuery] string subjectSearch)
        {
            return await _sentEmailsService.GetEmails(HttpContext.Session.GetInt32("userId").Value, subjectSearch);
        }
        public async Task<SentEmails> GetEmailById([FromQuery] string emailId)
        {
            return await _sentEmailsService.GetEmailById(emailId);
        }
    }
}
