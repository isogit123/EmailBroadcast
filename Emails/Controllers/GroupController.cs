using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using Emails.Filters;
using Emails.Models;
using Emails.Services;
using Emails.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Emails.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(CookieAuthorizationFilter))]
    public class GroupController : ControllerBase
    {
        IGroupService _groupService;
        ISentEmailsService _sentEmailsService;
        IMailWrapperService _mailWrapperService;
        public GroupController(IGroupService groupService, IMailWrapperService mailWrapperService, ISentEmailsService sentEmailsService)
        {
            _groupService = groupService;
            _mailWrapperService = mailWrapperService;
            _sentEmailsService = sentEmailsService;
        }
        public async Task<List<Groups>> Get()
        {
            try
            {
                string userId = HttpContext.User.Identity.Name;
                var groups = await _groupService.GetGroups(userId);
                return groups;
            }
            catch
            {
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Add([FromBody] Groups group)
        {
            if (ModelState.IsValid)
            {
                string userId = HttpContext.User.Identity.Name;
                await _groupService.AddGroup(group, userId);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Edit([FromBody] Groups group)
        {
            if (ModelState.IsValid)
            {
                string userId = HttpContext.User.Identity.Name;
                await _groupService.EditGroup(group, userId);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Delete([FromBody] GroupDeleteViewModel groupDeleteViewModel)
        {
            if (ModelState.IsValid)
            {
                string userId = HttpContext.User.Identity.Name;
                await _groupService.DeleteGroup(groupDeleteViewModel.GroupId, userId);
            }
        }
        public async Task<Groups> GetById([FromQuery] string groupId)
        {
            string userId = HttpContext.User.Identity.Name;
            return await _groupService.GetGroupById(groupId, userId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> SendMailToGroup([FromForm] EmailViewModel emailViewModel)
        {
            string userId = HttpContext.User.Identity.Name;
            string groupName = (await _groupService.GetGroupById(emailViewModel.GroupId, userId)).Name;
            var subject = emailViewModel.Subject ?? " ";
            var emails = await _groupService.GetGroupEmails(emailViewModel.GroupId, userId);
            var htmlContent = emailViewModel.HtmlContent ?? " ";
            var resp = await _mailWrapperService.SendMail(emails.ToArray(), $"{groupName} Group Broadcast", subject, htmlContent, emailViewModel.Attachments);
            if (resp != "-1")
            {
                await _sentEmailsService.AddEmails(new SentEmails
                {
                    IdFromMailService = resp,
                    GroupId = emailViewModel.GroupId,
                    Subject = subject,
                    GroupName = groupName
                }, userId);
                return "acc";
            }
            return "-1";
        }
    }
}
