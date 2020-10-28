using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    public class SentEmailsFailures
    {
        public int Id { get; set; }
        public string Recipient { get; set; }
        public string SentEmailsId { get; set; }
        [JsonIgnore]
        public virtual SentEmails SentEmails { get; set; }
    }
}
