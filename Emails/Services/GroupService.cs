using Emails.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.Services
{
    public class GroupService : IGroupService
    {
        private FirestoreDb _db;

        public GroupService(FirestoreDb firestoreDb)
        {
            _db = firestoreDb;
        }

        public async Task AddGroup(Groups group, string userId)
        {
            CollectionReference collectionReference = _db.Collection("users")
                .Document(userId).Collection("groups");
            group.Name = group.Name.Trim();
            group.Emails = group.Emails.Select(x => x.ToLower().Trim()).Distinct().ToList();
            group.Emails.Sort();
            await collectionReference.AddAsync(group);

        }

        public async Task DeleteGroup(string groupId, string userId)
        {
            DocumentReference documentReference = _db.Collection("users")
                .Document(userId).Collection("groups").Document(groupId);
            await documentReference.DeleteAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task EditGroup(Groups group, string userId)
        {
            Groups currentGroup = await GetGroupById(group.Id, userId);
            WriteBatch writeBatch = _db.StartBatch();
            CollectionReference collectionReference = _db.Collection("users")
                .Document(userId).Collection("groups");
            DocumentReference documentReference = collectionReference.Document(group.Id);
            group.Name = group.Name.Trim();
            group.Emails = group.Emails.Select(x => x.ToLower().Trim()).Distinct().ToList();
            group.Emails.Sort();
            writeBatch.Set(documentReference, group);
            if(currentGroup.Name != group.Name)
            {
                Dictionary<string, object> nameUpdate = new Dictionary<string, object>();
                nameUpdate["GroupName"] = group.Name;
                Query sentEmailsQuery = _db.Collection("users").Document(userId).Collection("sent_emails").WhereEqualTo("GroupId", group.Id);
                var snaps = await sentEmailsQuery.GetSnapshotAsync();
                foreach(DocumentSnapshot snap in snaps)
                {
                    writeBatch.Update(snap.Reference, nameUpdate);
                }
            }
            await writeBatch.CommitAsync();
        }

        public async Task<Groups> GetGroupById(string groupId, string userId)
        {
            DocumentReference documentReference = _db.Collection("users")
                .Document(userId).Collection("groups").Document(groupId);
            var snapshot = await documentReference.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Groups group = snapshot.ConvertTo<Groups>();
                return group;
            }
            return null;
        }

        public async Task<List<Groups>> GetGroups(string userId)
        {
            CollectionReference collectionReference = _db.Collection("users")
                .Document(userId).Collection("groups");
            var snapshot = await collectionReference.OrderBy("Name").GetSnapshotAsync();
            List<Groups> groups = new List<Groups>();
            foreach(var doc in snapshot.Documents)
            {
                Groups group = doc.ConvertTo<Groups>();
                groups.Add(group);
            }
            return groups;
        }

        public async Task<List<string>> GetGroupEmails(string groupId, string userId)
        {
            var group = await GetGroupById(groupId, userId);
            return group.Emails;
        }

    }
}
