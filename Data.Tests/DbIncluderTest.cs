using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace Data.Tests
{
    public class DbIncluderTest
    {
        [Fact]
        public void Include_UsesEF()
        {
            var i = new DbIncluder();
            var queryable = new List<TestModel>().AsQueryable();
            Assert.IsAssignableFrom<IIncludableQueryable<TestModel, int>>(i.Include(queryable, e => e.Id));
        }
    }
}
