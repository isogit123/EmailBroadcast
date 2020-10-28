using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Models
{
    //To store logout from all devices requests.
    public class LogOutRequests
    {
        [Key]
        public int SessionId { get; set; }
    }
}
