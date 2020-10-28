using Emails.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Emails.DB
{
    public class Context : DbContext
    {
        public DbSet<Groups> Groups { get; set; }
        public DbSet<Models.Emails> Emails { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Tokens> Tokens { get; set; }
        public DbSet<SentEmails> SentEmails { get; set; }
        public DbSet<SentEmailsFailures> SentEmailsFailures { get; set; }
        public DbSet<LogOutRequests> LogOutRequests { get; set; }
        public Context(DbContextOptions<Context> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Users>()
                .HasIndex(x => x.Email)
                .IsUnique();
            modelBuilder.Entity<SentEmailsFailures>()
                .HasIndex(x => new { x.Recipient, x.SentEmailsId })
                .IsUnique();
        }
    }
}
