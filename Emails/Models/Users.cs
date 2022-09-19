using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    [FirestoreData]
    public class Users
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [Required]
        [FirestoreProperty]
        public string Name { get; set; }
        [Required]
        [FirestoreProperty]
        public string Password { get; set; }
        [EmailAddress]
        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public bool IsEmailConfirmed { get; set; }
        [FirestoreProperty]
        public string CookieGUID { get; set; }
    }
}
