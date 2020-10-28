using Emails.DB;
using Emails.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public class GroupService : IGroupService
    {
        private Context _db;

        public GroupService(Context db)
        {
            _db = db;
        }

        public async Task AddGroup(Groups group, int userId)
        {
            group.UsersId = userId;
            await _db.Groups.AddAsync(group);
            await _db.SaveChangesAsync();

        }

        public async Task DeleteGroup(int groupId)
        {
            _db.Groups.Remove(_db.Groups.Find(groupId));
            await _db.SaveChangesAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task EditGroup(Groups group)
        {
            _db.Emails.RemoveRange(_db.Emails.Where(e => e.GroupsId == group.Id));
            _db.Groups.Update(group);
            await _db.SaveChangesAsync();
        }

        public async Task<Groups> GetGroupById(int groupId)
        {
            return await _db.Groups.Where(e => e.Id == groupId).Include(x => x.Emails).FirstOrDefaultAsync();
        }

        public async Task<List<Groups>> GetGroups(int userId)
        {
            return await _db.Groups.Where(e => e.UsersId == userId).OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<List<Models.Emails>> GetGroupEmails(int groupId)
        {
            return await _db.Emails.Where(e => e.GroupsId == groupId).OrderBy(e => e.Email).ToListAsync();
        }

    }
}
