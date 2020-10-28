using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Emails.Models
{
    public class Emails
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public int GroupsId { get; set; }
        [JsonIgnore]
        public virtual Groups Groups { get; set; }
    }
}
