using Data.Repositories.ReadOnly;
using Data.Repositories.ReadOnly.Utility;
using Data.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Tests
{
    [TestClass]
    public class ReadOnlyRepositoryTest
    {
        public ReadOnlyRepository GetRepoWithData<T>(List<T> data, string name) where T : ViewEntity
        {
            var logger = Mock.Of<ILogger>();
            var dc = new ReadOnlyDataContext(test: true, testName: name);
            data.ForEach(c => dc.Add(c));
            dc.SaveChanges();

            var repo = new ReadOnlyRepository(dc);
            return repo;
        }

        public Mock<ReadOnlyRepository> GetRepoMock<T>(string name, bool setupIncl = false) where T : ViewEntity
        {
            var logger = Mock.Of<ILogger>();
            var dc = new ReadOnlyDataContext(test: true, testName: name);
            var repoM = new Mock<ReadOnlyRepository>(dc);
            if (setupIncl)
                repoM.Setup(m => m.Include(It.IsAny<IQueryable<T>>(), It.IsAny<Expression<Func<T, object>>>()))
                    .Returns(dc.ISet<T>())
                    .Verifiable();
            return repoM;
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void Page_SortsAscAsRequested()
        {
            // Setup
            var v = new List<V_Population>()
            {
                new V_Population() { UserId = "1"},
                new V_Population() { UserId = "3"},
                new V_Population() { UserId = "2"},
            };
            var repo = GetRepoWithData(v, nameof(Page_SortsAscAsRequested));
            var a = new[] { new SortSpecification("UserId", SortDirection.Ascending), };
            var d = new Dictionary<string, Expression<Func<V_Population, object>>>()
            {
                { "UserId", t => t.UserId }
            };
            var f = new PopulationSortFactory<V_Population>(a, d);

            // Test
            var results = repo.Page<V_Population>(
                Specification<V_Population>.All(), f, 0, 3);

            // Assert
            Assert.IsTrue(v.OrderBy(c => c.UserId).SequenceEqual(results, new ReadOnlyEqComparer<V_Population>()));
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void Page_SortsDescAsRequested()
        {
            var r = new List<V_Population>()
            {
                new V_Population() { UserId = "1" },
                new V_Population() { UserId = "3" },
                new V_Population() { UserId = "2" },
            };
            var repo = GetRepoWithData(r, nameof(Page_SortsDescAsRequested));
            var a = new[] {new SortSpecification("UserId", SortDirection.Descending),};
            var d = new Dictionary<string, Expression<Func<V_Population, object>>>()
            {
                { "UserId", t => t.UserId }
            };
            var f = new PopulationSortFactory<V_Population>(a, d);
            var results = repo.Page(Specification<V_Population>.All(), f, 0, 3);

            Assert.IsTrue(r.OrderByDescending(c => c.UserId).SequenceEqual(results, new ReadOnlyEqComparer<V_Population>()));
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void Page_Pages()
        {
            var r = new List<V_Population>()
            {
                new V_Population() { UserId = "1" },
                new V_Population() { UserId = "2" },
                new V_Population() { UserId = "3" },
                new V_Population() { UserId = "4" },
                new V_Population() { UserId = "5" },
            };
            var repo = GetRepoWithData(r, nameof(Page_Pages));
            var a = new[] { new SortSpecification("UserId", SortDirection.Ascending), };
            var d = new Dictionary<string, Expression<Func<V_Population, object>>>()
            {
                { "UserId", t => t.UserId }
            };
            var f = new PopulationSortFactory<V_Population>(a, d);

            var results = repo.Page(Specification<V_Population>.All(), f, 1, 2);

            Assert.AreEqual("3", results.First().UserId);
            Assert.AreEqual("4", results.Skip(1).First().UserId);
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void Page_Filters()
        {
            var r = new List<V_Population>()
            {
                new V_Population() { UserId = "1"},
                new V_Population() { UserId = "2"},
                new V_Population() { UserId = "3"},
                new V_Population() { UserId = "4"},
                new V_Population() { UserId = "5"},
            };
            var repo = GetRepoWithData(r, nameof(Page_Filters));
            var a = new[] { new SortSpecification("UserId", SortDirection.Ascending), };
            var d = new Dictionary<string, Expression<Func<V_Population, object>>>()
            {
                { "UserId", t => t.UserId }
            };
            var f = new PopulationSortFactory<V_Population>(a, d);

            var results = repo.Page<V_Population>(
                Specification<V_Population>.Start(
                    c => c.UserId == "3" || c.UserId == "5"), f, 0, 2);

            Assert.AreEqual("3", results.First().UserId);
            Assert.AreEqual("5", results.Skip(1).First().UserId);
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void Page_Includes()
        {
            var a = new[] { new SortSpecification("UserId", SortDirection.Descending), };
            var d = new Dictionary<string, Expression<Func<V_Population, object>>>()
            {
                { "UserId", t => t.UserId }
            };
            var f = new PopulationSortFactory<V_Population>(a, d);
            var repoM = GetRepoMock<V_Population>(nameof(Page_Includes));
            repoM.Object.Page(Specification<V_Population>.All(), f, includes: r => r.UserId);
            repoM.Verify();
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void FindOne_Includes()
        {
            var repoM = GetRepoMock<V_Population>(nameof(FindOne_Includes), setupIncl: true);
            repoM.Object.FindOne(Specification<V_Population>.All(), includes: r => r.UserId);
            repoM.Verify();
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void FindOne_Filters()
        {
            var r = new List<V_Population>()
            {
                new V_Population() { UserId = "1" },
                new V_Population() { UserId = "2" },
                new V_Population() { UserId = "3" },
                new V_Population() { UserId = "4" },
                new V_Population() { UserId = "5" },
            };
            var repo = GetRepoWithData(r, nameof(FindOne_Filters));

            var result = repo.FindOne(Specification<V_Population>.Start(c => c.UserId == "3"));

            Assert.AreEqual(result.UserId, "3");
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void FindAll_Includes()
        {
            var repoM = GetRepoMock<V_Population>(nameof(FindAll_Includes), setupIncl: true);
            repoM.Object.FindAll(Specification<V_Population>.All(), includes: r => r.UserId);
            repoM.Verify();
        }

        [TestMethod, TestCategory("ReadOnlyRepository")]
        public void FindAll_Filters()
        {
            var r = new List<V_Population>()
            {
                new V_Population() { UserId = "1" },
                new V_Population() { UserId = "2" },
                new V_Population() { UserId = "3" },
                new V_Population() { UserId = "4" },
                new V_Population() { UserId = "5" },
            };
            var repo = GetRepoWithData(r, nameof(FindAll_Filters));

            var results = repo.FindAll(Specification<V_Population>.Start(c => string.Compare(c.UserId, "3") > 0));

            Assert.AreEqual("4", results.First().UserId);
            Assert.AreEqual("5", results.Skip(1).First().UserId);
        }
    }
}
