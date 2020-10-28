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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Emails.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(AuthenticationFilter))]
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
                var groups = await _groupService.GetGroups(HttpContext.Session.GetInt32("userId").Value);
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
                await _groupService.AddGroup(group, HttpContext.Session.GetInt32("userId").Value);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Edit([FromBody] Groups group)
        {
            if (ModelState.IsValid)
            {
                group.UsersId = HttpContext.Session.GetInt32("userId").Value;
                await _groupService.EditGroup(group);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Delete([FromBody] GroupDeleteViewModel groupDeleteViewModel)
        {
            await _groupService.DeleteGroup(groupDeleteViewModel.GroupId);
        }
        public async Task<Groups> GetById([FromQuery] int groupId)
        {
            return await _groupService.GetGroupById(groupId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> SendMailToGroup([FromForm] EmailViewModel emailViewModel)
        {
            string groupName = (await _groupService.GetGroupById(emailViewModel.GroupId)).Name;
            var subject = emailViewModel.Subject ?? " ";
            var emails = await _groupService.GetGroupEmails(emailViewModel.GroupId);
            var htmlContent = emailViewModel.HtmlContent ?? " ";
            var resp = await _mailWrapperService.SendMail(emails.Select(x => x.Email).ToArray(), $"{groupName} Group Broadcast", subject, htmlContent, emailViewModel.Attachments);
            if (resp != "-1")
            {
                await _sentEmailsService.AddEmails(new SentEmails
                {
                    Id = resp,
                    GroupsId = emailViewModel.GroupId,
                    Subject = subject
                });
                return "acc";
            }
            return "-1";
        }
    }
}
