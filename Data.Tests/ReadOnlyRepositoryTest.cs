using Data.Repositories.ReadOnly;
using Data.Utilities;
using Moq;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Data.Models;
using Xunit;

namespace Data.Tests
{
    public class ReadOnlyRepositoryTest
    {
        protected ReadOnlyRepository<string> GetRepoWithData<T>(List<T> data, string name)
            where T : class, IEntity<string>
        {
            var logger = Mock.Of<ILogger>();
            var dc = new ReadOnlyDataContext("connstr", test: true, testName: name);
            data.ForEach(c => dc.Add(c));
            dc.SaveChanges();

            var repo = new ReadOnlyRepository<string>(dc);
            return repo;
        }

        protected Mock<ReadOnlyRepository<string>> GetRepoMock<T>(string name, bool setupIncl = false)
            where T : class, IEntity<string>
        {
            var logger = Mock.Of<ILogger>();
            var dc = new ReadOnlyDataContext("connstr", test: true, testName: name);
            var repoM = new Mock<ReadOnlyRepository<string>>(dc);
            if (setupIncl)
                repoM.Setup(m => m.Include(It.IsAny<IQueryable<T>>(), It.IsAny<Expression<Func<T, object>>>()))
                    .Returns(dc.ISet<T>())
                    .Verifiable();
            return repoM;
        }

        [Fact]
        public void Page_SortsAscAsRequested()
        {
            // Setup
            var v = new List<V_MyView>()
            {
                new V_MyView() { Id = "1"},
                new V_MyView() { Id = "3"},
                new V_MyView() { Id = "2"},
            };
            var repo = GetRepoWithData(v, nameof(Page_SortsAscAsRequested));
            var a = new[] { new SortSpecification("Id", SortDirection.Ascending), };
            var d = new Dictionary<string, dynamic>()
            {
                { "Id", (Expression<Func<V_MyView, string>>)(t => t.Id) }
            };
            var f = new SortFactory<V_MyView, string>(a, d);

            // Test
            var results = repo.Page<V_MyView>(
                Specification<V_MyView>.All(), f, 0, 3);

            // Assert
            Assert.True(v.OrderBy(c => c.Id).SequenceEqual(results, new ReadOnlyEqComparer<V_MyView>()));
        }

        [Fact]
        public void Page_SortsDescAsRequested()
        {
            var r = new List<V_MyView>()
            {
                new V_MyView() { Id = "1" },
                new V_MyView() { Id = "3" },
                new V_MyView() { Id = "2" },
            };
            var repo = GetRepoWithData(r, nameof(Page_SortsDescAsRequested));
            var a = new[] {new SortSpecification("Id", SortDirection.Descending),};
            var d = new Dictionary<string, dynamic>()
            {
                { "Id", (Expression<Func<V_MyView, string>>)(t => t.Id) }
            };
            var f = new SortFactory<V_MyView, string>(a, d);
            var results = repo.Page(Specification<V_MyView>.All(), f, 0, 3);

            Assert.True(r.OrderByDescending(c => c.Id).SequenceEqual(results, new ReadOnlyEqComparer<V_MyView>()));
        }

        [Fact]
        public void Page_Pages()
        {
            var r = new List<V_MyView>()
            {
                new V_MyView() { Id = "1" },
                new V_MyView() { Id = "2" },
                new V_MyView() { Id = "3" },
                new V_MyView() { Id = "4" },
                new V_MyView() { Id = "5" },
            };
            var repo = GetRepoWithData(r, nameof(Page_Pages));
            var a = new[] { new SortSpecification("Id", SortDirection.Ascending), };
            var d = new Dictionary<string, dynamic>()
            {
                { "Id", (Expression<Func<V_MyView, string>>)(t => t.Id) }
            };
            var f = new SortFactory<V_MyView, string>(a, d);

            var results = repo.Page(Specification<V_MyView>.All(), f, 1, 2);

            Assert.Equal("3", results.First().Id);
            Assert.Equal("4", results.Skip(1).First().Id);
        }

        [Fact]
        public void Page_Filters()
        {
            var r = new List<V_MyView>()
            {
                new V_MyView() { Id = "1"},
                new V_MyView() { Id = "2"},
                new V_MyView() { Id = "3"},
                new V_MyView() { Id = "4"},
                new V_MyView() { Id = "5"},
            };
            var repo = GetRepoWithData(r, nameof(Page_Filters));
            var a = new[] { new SortSpecification("Id", SortDirection.Ascending), };
            var d = new Dictionary<string, dynamic>()
            {
                { "Id", (Expression<Func<V_MyView, string>>)(t => t.Id) }
            };
            var f = new SortFactory<V_MyView, string>(a, d);

            var results = repo.Page<V_MyView>(
                Specification<V_MyView>.Start(
                    c => c.Id == "3" || c.Id == "5"), f, 0, 2);

            Assert.Equal("3", results.First().Id);
            Assert.Equal("5", results.Skip(1).First().Id);
        }

        [Fact]
        public void Page_Includes()
        {
            var a = new[] { new SortSpecification("Id", SortDirection.Descending), };
            var d = new Dictionary<string, dynamic>()
            {
                { "Id", (Expression<Func<V_MyView, string>>)(t => t.Id) }
            };
            var f = new SortFactory<V_MyView, string>(a, d);
            var repoM = GetRepoMock<V_MyView>(nameof(Page_Includes));
            repoM.Object.Page(Specification<V_MyView>.All(), f, includes: r => r.Id);
            repoM.Verify();
        }

        [Fact]
        public void FindOne_Includes()
        {
            var repoM = GetRepoMock<V_MyView>(nameof(FindOne_Includes), setupIncl: true);
            repoM.Object.FindOne(Specification<V_MyView>.All(), includes: r => r.Id);
            repoM.Verify();
        }

        [Fact]
        public void FindOne_Filters()
        {
            var r = new List<V_MyView>()
            {
                new V_MyView() { Id = "1" },
                new V_MyView() { Id = "2" },
                new V_MyView() { Id = "3" },
                new V_MyView() { Id = "4" },
                new V_MyView() { Id = "5" },
            };
            var repo = GetRepoWithData(r, nameof(FindOne_Filters));

            var result = repo.FindOne(Specification<V_MyView>.Start(c => c.Id == "3"));

            Assert.Equal(result.Id, "3");
        }

        [Fact]
        public void FindAll_Includes()
        {
            var repoM = GetRepoMock<V_MyView>(nameof(FindAll_Includes), setupIncl: true);
            repoM.Object.FindAll(Specification<V_MyView>.All(), includes: r => r.Id);
            repoM.Verify();
        }

        [Fact]
        public void FindAll_Filters()
        {
            var r = new List<V_MyView>()
            {
                new V_MyView() { Id = "1" },
                new V_MyView() { Id = "2" },
                new V_MyView() { Id = "3" },
                new V_MyView() { Id = "4" },
                new V_MyView() { Id = "5" },
            };
            var repo = GetRepoWithData(r, nameof(FindAll_Filters));

            var results = repo.FindAll(Specification<V_MyView>.Start(c => string.Compare(c.Id, "3") > 0));

            Assert.Equal("4", results.First().Id);
            Assert.Equal("5", results.Skip(1).First().Id);
        }
    }
}
