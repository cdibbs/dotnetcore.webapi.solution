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

        public DataContext(IPrincipal user, ILogger logger) : base()
        {
            this.User = user;
            this.Logger = logger;
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
            try
            {
                var un = this.User?.Identity?.Name;
                u = Users.FirstOrDefault(user => user.Username == un);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error loading user.");
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
