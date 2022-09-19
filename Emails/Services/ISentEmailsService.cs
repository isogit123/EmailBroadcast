using Emails.Models;
using Emails.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public interface ISentEmailsService
    {
        Task<ReturnSentEmailsViewModel> GetEmails(string userId, string subjectSearch, DateTime? sendingDateStart = null);
        Task AddEmails(SentEmails sentEmails, string userId);

        Task AddEmailFailure(string userId, string emailId, string failedToReachEmail);
        Task<SentEmails> GetEmailById(string emailId, string userId);

    }
}
