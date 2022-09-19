using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    [FirestoreData]
    public class Tokens
    {
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public string Token { get; set; }
        [FirestoreProperty]
        public DateTime GenerationDate { get; set; }
    }
}
