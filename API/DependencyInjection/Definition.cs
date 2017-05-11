using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DependencyInjection
{
    public class Definition<Context>
    {
        string Name;
        Func<Context, Object> Func;
        public Definition(string name = null)
        {
            this.Name = name;
        }

        public Definition(Type from, Func<Context, object> func, string name = null)
        {
            this.Func = func;
            this.Name = name;
        }

        /*public Definition(Type from, Func<Context, struct> func, string name = null)
        {
            this.Func = func;
            this.Name = name;
        }*/

        public Definition(Type from, Type to, string name = null)
        {
            this.Name = name;
        }
    }
}
