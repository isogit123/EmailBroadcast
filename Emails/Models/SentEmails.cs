using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    [FirestoreData]
    public class SentEmails
    {
        [FirestoreProperty]
        public string IdFromMailService { get; set; }
        [FirestoreProperty]
        public string Subject { get; set; }
        [FirestoreProperty]
        public DateTime SendingDate { get; set; }
        [FirestoreProperty]
        public string GroupId { get; set; }
        [FirestoreProperty]
        public string GroupName { get; set; }
        [FirestoreProperty]
        public List<string> FailedToReachEmails { get; set; } = new List<string>();
    }
}
