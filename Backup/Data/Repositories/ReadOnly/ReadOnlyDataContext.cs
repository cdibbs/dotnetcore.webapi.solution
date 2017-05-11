using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.ReadOnly
{
    public class ReadOnlyDataContext : DbContext, IReadOnlyDataContext
    {
        public DbSet<V_Population> Population { get; set; }

        public DbSet<T> ISet<T>() where T : class => Set<T>();
    }
}
