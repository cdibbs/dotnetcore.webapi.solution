using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console.Library.Deltifiers
{
    public abstract class BaseSetDeltifier<TSource, TTarget, TResult, TKey> : ISetDeltifier<TSource, TTarget, TResult, TKey>
    {
        protected IEnumerable<TResult> results = new LinkedList<TResult>();

        public virtual IEnumerable<TResult> ProcessDeltas(
            IDictionary<TKey, TSource> source,
            IDictionary<TKey, TTarget> target)
        {
            foreach (var s in source)
            {
                if (target.ContainsKey(s.Key))
                {
                    var t = target[s.Key];
                    if (Different(s.Value, t))
                    {
                        WhenExistsAndDifferent(s.Value, t);
                    }
                    else
                    {
                        WhenExistsAndSame(s.Value, t);
                    }
                }
                else
                {
                    WhenOnlyExistsInSource(s.Value);
                }
            }

            foreach (var t in target)
            {
                if (!source.ContainsKey(t.Key))
                {
                    WhenOnlyExistsInTarget(t.Value);
                }
            }

            return results;
        }

        public virtual bool Different(TSource source, TTarget target) => ! source.Equals(target);

        public abstract void WhenOnlyExistsInSource(TSource source);
        public abstract void WhenExistsAndDifferent(TSource source, TTarget target);
        public abstract void WhenExistsAndSame(TSource source, TTarget target);
        public abstract void WhenOnlyExistsInTarget(TTarget target);
    }
}
