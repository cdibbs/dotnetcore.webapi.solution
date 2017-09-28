using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace API.Models.ViewModels
{
    public interface IViewModel<T> : IViewModel<T, long> where T: IEntity<long>
    {
        long Id { get; set; }
    }

    public interface IViewModel<T, TKey>
    {
        TKey Id { get; set; }
    }
}
