using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    [FirestoreData]
    public class Groups
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [Required]
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public List<string> Emails { get; set; }
    }
}
