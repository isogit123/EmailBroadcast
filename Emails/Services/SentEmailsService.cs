using Emails.DB;
using Emails.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public class SentEmailsService : ISentEmailsService
    {
        private Context _db;

        public SentEmailsService(Context db)
        {
            _db = db;
        }
        public async Task AddEmails(SentEmails sentEmails)
        {
            sentEmails.SendingDate = DateTime.UtcNow;
            await _db.SentEmails.AddAsync(sentEmails);
            await _db.SaveChangesAsync();
        }
        public async Task AddEmailFailure(SentEmailsFailures sentEmailsFailures)
        {
            await _db.SentEmailsFailures.AddAsync(sentEmailsFailures);
            await _db.SaveChangesAsync();
        }

        public async Task<SentEmails> GetEmailById(string emailId)
        {
            SentEmails email = await _db.SentEmails.Where(e => e.Id == emailId).Include(e => e.Groups).FirstAsync();
            email.SentEmailsFailures = await _db.SentEmailsFailures.Where(e => e.SentEmailsId == emailId).OrderBy(e => e.Recipient).ToListAsync();
            return email;
        }

        public async Task<List<SentEmails>> GetEmails(int userId, string subjectSearch)
        {
            IQueryable<SentEmails> exp = _db.SentEmails.Where(e => e.Groups.UsersId == userId).Select(e => new SentEmails
            {
                Id = e.Id,
                Subject = e.Subject,
                Groups = e.Groups,
                SendingDate = e.SendingDate,
                SentEmailsFailuresCount = e.SentEmailsFailures.Count()
            });
            if (!string.IsNullOrEmpty(subjectSearch))
            {
                exp = exp.Where(e => e.Subject.Contains(subjectSearch));
            }
            return await exp.OrderByDescending(e => e.SendingDate).ToListAsync();
        }

    }
}
