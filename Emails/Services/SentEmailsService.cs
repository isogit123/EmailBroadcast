using Emails.Models;
using Emails.ViewModels;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public class SentEmailsService : ISentEmailsService
    {
        private FirestoreDb _db;

        public SentEmailsService(FirestoreDb db)
        {
            _db = db;
        }
        public async Task AddEmails(SentEmails sentEmails, string userId)
        {
            CollectionReference collectionReference = _db.Collection("users").Document(userId).Collection("sent_emails");
            sentEmails.SendingDate = DateTime.UtcNow;
            await collectionReference.AddAsync(sentEmails);
        }
        public async Task AddEmailFailure(string userId, string emailId, string failedToReachEmail)
        {
            Query query = _db.Collection("users").Document(userId).Collection("sent_emails")
                .WhereEqualTo("IdFromMailService", emailId);
            var snap = await query.GetSnapshotAsync();
            var reference = snap.First().Reference;
            await reference.UpdateAsync("FailedToReachEmails", FieldValue.ArrayUnion(failedToReachEmail));
        }

        public async Task<SentEmails> GetEmailById(string emailId, string userId)
        {
            Query query = _db.Collection("users").Document(userId).Collection("sent_emails")
                .WhereEqualTo("IdFromMailService", emailId);
            var snap = await query.GetSnapshotAsync();
            SentEmails email = snap.First().ConvertTo<SentEmails>();
            email.FailedToReachEmails.Sort();
            return email;
        }

        public async Task<ReturnSentEmailsViewModel> GetEmails(string userId, string subjectSearch, DateTime? sendingDateStart = null)
        {
            //TODO Implement subject search.
            int quantity = 11;
            Query query;
            if (sendingDateStart.HasValue)
                query = _db.Collection("users").Document(userId).Collection("sent_emails").OrderByDescending("SendingDate").StartAfter(sendingDateStart.Value.ToUniversalTime()).Limit(quantity);
            else
                query = _db.Collection("users").Document(userId).Collection("sent_emails").OrderByDescending("SendingDate").Limit(quantity);
            var snap = await query.GetSnapshotAsync();
            List<SentEmails> emails = new List<SentEmails>();
            foreach (var doc in snap.Documents)
            {
                SentEmails email = doc.ConvertTo<SentEmails>();
                emails.Add(email);
            }
            ReturnSentEmailsViewModel returnSentEmailsViewModel = new ReturnSentEmailsViewModel
            {
                Emails = emails.Count < 10? emails : emails.GetRange(0, quantity - 1),
                HasMore = emails.Count == 11
            };
            return returnSentEmailsViewModel;
        }

    }
}
