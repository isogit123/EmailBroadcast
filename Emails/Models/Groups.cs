using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    public class Groups
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Emails> Emails { get; set; }
        public int UsersId { get; set; }
        [JsonIgnore]
        public virtual Users Users { get; set; }
    }
}
