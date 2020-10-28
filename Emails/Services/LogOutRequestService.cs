using Emails.DB;
using Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public class LogOutRequestService : ILogOutRequestsService
    {
        private Context _db;
        public LogOutRequestService(Context db)
        {
            _db = db;
        }
        public async Task AddLogOutRequest(LogOutRequests logOutRequests)
        {
            await _db.LogOutRequests.AddAsync(logOutRequests);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsUserLogOutRequested(int userId)
        {
            LogOutRequests logOutRequest = await _db.LogOutRequests.FindAsync(userId);
            if (logOutRequest == null)
                return false;
            _db.LogOutRequests.Remove(logOutRequest);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
