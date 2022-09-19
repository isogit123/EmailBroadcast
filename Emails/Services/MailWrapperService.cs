using Emails.Models;
using Emails.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Microsoft.Extensions.Configuration;

namespace Emails.Services
{
    public class MailWrapperService : IMailWrapperService
    {
        private readonly string senderEmail;
        private readonly string emailApiKey;
        public MailWrapperService()
        {
            emailApiKey = Environment.GetEnvironmentVariable("EmailApiKey");
            senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
        }
        public async Task<string> SendMail(string[] to, string displayName, string subject, string body, List<IFormFile> attachments)
        {
            Configuration.Default.AddApiKey("api-key", emailApiKey);
            var apiInstance = new SMTPApi();
            SendSmtpEmailSender sender = new SendSmtpEmailSender(displayName, senderEmail);
            List<SendSmtpEmailTo> too = new List<SendSmtpEmailTo>();
            foreach (var email in to)
                too.Add(new SendSmtpEmailTo(email));
            var sendSmtpEmail = new SendSmtpEmail(sender, too);
            sendSmtpEmail.Subject = subject;
            sendSmtpEmail.HtmlContent = body;
            sendSmtpEmail.TextContent = body;
            if (attachments != null)
            {
                sendSmtpEmail.Attachment = new List<SendSmtpEmailAttachment>();
                long attachmentsTotalSize = 0;
                foreach (IFormFile attachment in attachments)
                {
                    MemoryStream st = new MemoryStream();
                    await attachment.CopyToAsync(st);
                    attachmentsTotalSize += st.Length;
                    if (attachmentsTotalSize > 10485760) //Attachments Size should not exceed 10 MB
                        return "-1";
                    sendSmtpEmail.Attachment.Add(new SendSmtpEmailAttachment(null, st.ToArray(), attachment.FileName));
                }
            }

            CreateSmtpEmail result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            return result.MessageId;
        }
    }
}
