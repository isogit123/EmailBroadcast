using Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public interface IGroupService
    {
        Task<List<Groups>> GetGroups(string userId);
        Task<Groups> GetGroupById(string groupId, string userId);
        Task AddGroup(Groups group, string userId);
        Task EditGroup(Groups group, string userId);
        Task DeleteGroup(string groupId, string userId);
        Task<List<string>> GetGroupEmails(string groupId, string userId);
    }
}
