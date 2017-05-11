using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AllDataContext : DbContext, ISoftDeletedDataContext
    {
        public AllDataContext(DbContextOptions opts = null) : base(opts)
        { }

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
