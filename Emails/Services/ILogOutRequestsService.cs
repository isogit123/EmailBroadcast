using Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public interface ILogOutRequestsService
    {
        Task AddLogOutRequest(LogOutRequests logOutRequests);
        Task<bool> IsUserLogOutRequested(int userId);
    }
}
