using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emails.Filters;
using Emails.Models;
using Emails.Services;
using Emails.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Emails.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(CookieAuthorizationFilter))]
    public class SentEmailsController : ControllerBase
    {
        ISentEmailsService _sentEmailsService;
        public SentEmailsController(ISentEmailsService sentEmailsService)
        {
            _sentEmailsService = sentEmailsService;
        }

        public async Task<ReturnSentEmailsViewModel> GetEmails([FromQuery] GetSentEmailsViewModel getSentEmailsViewModel)
        {
            string userId = HttpContext.User.Identity.Name;
            return await _sentEmailsService.GetEmails(userId, "", getSentEmailsViewModel.SendingDateStart);
        }
        public async Task<SentEmails> GetEmailById([FromQuery] string emailId)
        {
            string userId = HttpContext.User.Identity.Name;
            return await _sentEmailsService.GetEmailById(emailId, userId);
        }
    }
}
