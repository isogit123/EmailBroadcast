using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    public class SentEmails
    {
        [Key]
        public string Id { get; set; }
        public string Subject { get; set; }
        public DateTime SendingDate { get; set; }
        public int GroupsId { get; set; }
        public virtual Groups Groups { get; set; }
        public virtual List<SentEmailsFailures> SentEmailsFailures { get; set; }
        [NotMapped]
        public int SentEmailsFailuresCount { get; set; }
    }
}
