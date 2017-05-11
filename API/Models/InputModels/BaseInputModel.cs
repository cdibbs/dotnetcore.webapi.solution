using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.InputModels
{
    public class BaseInputModel<T> : IInputModel<T>
        where T : IEntity
    {
        public long Id { get; set; }
    }
}
