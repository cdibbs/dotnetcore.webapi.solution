using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Data.Utilities;
using Xunit;

namespace Data.Tests
{
    public class SortFactoryTest
    {
        [Fact]
        public void ApplySorts_UsesDefaultWhenNoSorts()
        {
            var factory = new SortFactory<User, long>(new SortSpecification[0], new Dictionary<string, dynamic>());
            var list = new List<User>()
            {
                new User() {Id = 3},
                new User() {Id = 1},
                new User() {Id = 4}
            }.AsQueryable();

            var sorted = factory.ApplySorts(list);

            Assert.Equal(1, sorted.First().Id);
            Assert.Equal(3, sorted.ElementAt(1).Id);
            Assert.Equal(4, sorted.Last().Id);
        }

        [InlineData(SortDirection.Descending, "b", "a")]
        [InlineData(SortDirection.Ascending, "a", "b")]
        [Theory]
        public void ApplySorts_AppliesAllSorts(SortDirection dir, string expFirst1, string expFirst2)
        {
            var specs = new SortSpecification[]
            {
                new SortSpecification("Id", SortDirection.Ascending), 
                new SortSpecification("First", dir), 
            };
            var available = new Dictionary<string, dynamic>()
            {
                { "Id", (Expression<Func<User, long>>)(u => u.Id) },
                { "First", (Expression<Func<User, string>>)(u => u.First) }
            };

            var factory = new SortFactory<User, long>(specs, available);
            var list = new List<User>()
            {
                new User() {Id = 3, First = expFirst1},
                new User() {Id = 3, First = expFirst2},
                new User() {Id = 1},
                new User() {Id = 4}
            }.AsQueryable();

            var sorted = factory.ApplySorts(list);

            Assert.Equal(1, sorted.First().Id);
            Assert.Equal(expFirst1, sorted.ElementAt(1).First);
            Assert.Equal(expFirst2, sorted.ElementAt(2).First);
            Assert.Equal(4, sorted.Last().Id);
        }

        [Fact]
        public void GetLambda_ThrowsOnMissingFieldname()
        {
            var factory = new SortFactory<User, long>(new SortSpecification[0], new Dictionary<string, dynamic>());
            var ex = Assert.Throws<ArgumentException>(() => factory.GetLambda("bogus"));
            Assert.Contains("does not exist in provided selectors", ex.Message);
        }
    }
}
