using System.Linq;

namespace Data.Utilities
{
    public interface ISortFactory<T> where T : IEntity
    {
        IOrderedQueryable<T> ApplySorts(IQueryable<T> queryable);
    }
}