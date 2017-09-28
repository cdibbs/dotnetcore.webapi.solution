using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.InputModels
{
    public class BaseInputModel<T, TKey> : IInputModel<T, TKey>
        where TKey: IComparable
        where T : IEntity<TKey>
    {
        public long Id { get; set; }
    }
}
