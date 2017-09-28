using System;

namespace Data
{
    public interface IEntity<T> where T: IComparable
    {
        T Id { get; set; }
    }
}