using System;

namespace Emails.ViewModels
{
    public class GetSentEmailsViewModel
    {
        public string SubjectSearch { get; set; }
        public DateTime? SendingDateStart { get; set; }
    }
}
