using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DependencyInjection
{
    public class DefinitionFactory<Context>
    {
        /*public Definition<Context> Define<From, To>(Func<Context, To> func, string name = null) where To: struct
        {
            //return new Definition<Context>(typeof(From), func, name);
        }*/

        public Definition<Context> Define<From>(Type to, string name = null)
        {
            return new Definition<Context>(typeof(From), to, name);
        }
    }
}
