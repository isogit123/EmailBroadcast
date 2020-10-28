using Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public interface ISentEmailsService
    {
        Task<List<SentEmails>> GetEmails(int userId, string subjectSearch);
        Task AddEmails(SentEmails sentEmails);

        Task AddEmailFailure(SentEmailsFailures sentEmailsFailures);
        Task<SentEmails> GetEmailById(string emailId);

    }
}
