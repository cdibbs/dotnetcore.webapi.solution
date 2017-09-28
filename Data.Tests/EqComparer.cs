using Data.Repositories.ReadOnly;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Tests
{
    public class EqComparer<T> : EqualityComparer<T> where T : BaseEntity
    {
        public override bool Equals(T x, T y)
        {
            return x.Id == y.Id;
        }

        public override int GetHashCode(T obj)
        {
            throw new NotImplementedException();
        }
    }

    public class ReadOnlyEqComparer<T> : EqualityComparer<T> where T : IEntity<string>
    {
        public override bool Equals(T x, T y)
        {
            return x.Id == y.Id;
        }

        public override int GetHashCode(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
