using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DependencyInjection
{
    public class InjectAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
