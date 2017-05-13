using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;
using System.Security.Principal;

namespace Data
{
    public class DataContext : DbContext, IDataContext
    {
        private IPrincipal User { get; set; }
        private ILogger Logger { get; set; }
        private bool Test;
        private string TestName;

        public DataContext(IPrincipal user, ILogger logger, bool test = false, string testName = "") : base()
        {
            this.User = user;
            this.Logger = logger;
            this.Test = test;
            this.TestName = testName;

        }

        protected override void OnConfiguring(DbContextOptionsBuilder opts)
        {
            if (this.Test)
                opts.UseInMemoryDatabase(TestName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Lazy Loading is unimplemented in EF Core as of 1.1.1 (2017-05-04).
            //this.Configuration.LazyLoadingEnabled = false;

            ///-- From EntityFramework.DynamicFilters. Not even a possibility until EF Core lifecycle hooks implemented:
            ///-- https://github.com/jcachat/EntityFramework.DynamicFilters/issues/48
            ///-- https://github.com/aspnet/EntityFramework/issues/626
            //modelBuilder.Filter("IsDeleted", (BaseEntity e) => e.IsDeleted, false);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }
      
        public DbSet<T> ISet<T>() where T: class => Set<T>();

        public void Save()
        {
            User u = null;
            var username = this.User?.Identity?.Name;
            try
            {               
                u = Users.FirstOrDefault(user => user.Username == username);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error loading user: {username}.");
            }

            if (u == null)
            {
                u = new User() {Id = 0};
            }

            foreach (var change in ChangeTracker.Entries())
            {
                var e = change.Entity as BaseEntity;
                if (change.State == EntityState.Added)
                {
                    if (e != null)
                    {
                        e.Created = DateTime.Now;
                        e.LastUpdated = DateTime.Now;
                        e.LastUpdatedBy = u.Id;

                    }
                } else if (change.State == EntityState.Modified)
                {
                    e.LastUpdated = DateTime.Now;
                    e.LastUpdatedBy = u.Id;
                }
            }

            this.SaveChanges();
        }
    }
}
