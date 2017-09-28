using System;
using Data;

namespace API.Models.InputModels
{
    public interface IInputModel<T, TKey>
        where TKey: IComparable
        where T: IEntity<TKey>
    {
        long Id { get; set; }
    }
}