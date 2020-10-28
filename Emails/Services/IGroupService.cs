using Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public interface IGroupService
    {
        Task<List<Groups>> GetGroups(int userId);
        Task<Groups> GetGroupById(int groupId);
        Task AddGroup(Groups group, int userId);
        Task EditGroup(Groups group);
        Task DeleteGroup(int groupId);
        Task<List<Models.Emails>> GetGroupEmails(int groupId);
    }
}
