using Emails.Models;
using System.Collections.Generic;

namespace Emails.ViewModels
{
    public class ReturnSentEmailsViewModel
    {
        public List<SentEmails> Emails { get; set; }
        public bool HasMore { get; set; }
    }
}
