using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    public class Tokens
    {
        public int UserId { get; set; }
        [Key]
        public string Token { get; set; }
        public DateTime GenerationDate { get; set; }
    }
}
