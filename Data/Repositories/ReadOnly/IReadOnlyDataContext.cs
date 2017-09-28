using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.ReadOnly
{
    public interface IReadOnlyDataContext
    {
        DbSet<V_MyView> V_NullMembershipEligibilityMappings { get; set; }
        DbSet<T> ISet<T>() where T : class;
    }
}