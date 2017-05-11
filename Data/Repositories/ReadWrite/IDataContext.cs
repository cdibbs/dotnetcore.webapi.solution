using Microsoft.EntityFrameworkCore;

namespace Data
{
    public interface IDataContext : IUnitOfWork
    {
        DbSet<User> Users { get; set; }

        DbSet<Role> Roles { get; set; }

        DbSet<UserRole> UserRoles { get; set; }

        int SaveChanges();

        DbSet<T> ISet<T>() where T : class;
    }
}
