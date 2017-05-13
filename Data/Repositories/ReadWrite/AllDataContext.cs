using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AllDataContext : DbContext, ISoftDeletedDataContext
    {
        private bool Test;
        private string TestName;

        public AllDataContext(bool test = false, string testName = "")
        {
            this.Test = test;
            this.TestName = testName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder opts)
        {
            if (this.Test)
                opts.UseInMemoryDatabase(TestName);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }
        
        public DbSet<T> ISet<T>() where T: class => Set<T>();

        public void Save()
        {
            this.SaveChanges();
        }
    }
}
