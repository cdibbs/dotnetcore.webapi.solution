using System.Collections.Generic;

namespace Console.Library.Deltifiers
{
    public interface ISetDeltifier<TSource, TTarget, TResult, TKey>
    {
        IEnumerable<TResult> ProcessDeltas(
            IDictionary<TKey, TSource> source,
            IDictionary<TKey, TTarget> target);
    }
}