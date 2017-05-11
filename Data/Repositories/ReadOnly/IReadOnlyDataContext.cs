using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.ReadOnly
{
    public interface IReadOnlyDataContext
    {
        DbSet<V_Population> Population { get; set; }
        DbSet<T> ISet<T>() where T : class;
    }
}