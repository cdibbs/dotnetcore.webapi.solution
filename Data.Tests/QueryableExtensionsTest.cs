using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Data.QueryableExtensions;
using Moq;
using Xunit;

namespace Data.Tests
{
    public class QueryableExtensionsTest
    {
        static QueryableExtensionsTest()
        {
            QueryableExtensions.QueryableExtensions.Includer = Mock.Of<QueryableExtensions.IIncluder>();
        }

        [Fact]
        public void Include_UsesIncluderInclude()
        {
            var queryable = new List<TestModel>().AsQueryable();
            queryable.Include(q => q.Id);
            Mock.Get(QueryableExtensions.QueryableExtensions.Includer)
                .Verify(i => i.Include(It.Is<IQueryable<TestModel>>(q => q == queryable), It.IsAny<Expression<Func<TestModel, int>>>()), Times.Once);
        }

        [Fact]
        public void NullIncluder_NullOp()
        {
            var i = new QueryableExtensions.NullIncluder();
            var queryable = new List<TestModel>().AsQueryable();
            Assert.Equal(queryable, i.Include(queryable, e => e.Id));
        }
    }
}
