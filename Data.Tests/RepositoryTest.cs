using Data.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace Data.Tests
{
    [TestClass]
    public class RepositoryTest
    {
        public Repository GetRepoWithData<T>(List<T> data, string name) where T : BaseEntity
        {
            var logger = Mock.Of<ILogger>();
            var dc = new DataContext("test", logger, test: true, testName: $"FilterByIsDeleted{name}");
            var noDeletesDc = new AllDataContext(test: true, testName: $"Unfiltered{name}");

            data.ForEach(c => dc.Add(c));
            data.ForEach(c => { if (!c.IsDeleted) noDeletesDc.Add(c); });
            dc.SaveChanges();
            noDeletesDc.SaveChanges();

            var repo = new Repository(dc, noDeletesDc);
            return repo;
        }

        public Mock<Repository> GetRepoMock<T>(string name, bool setupNoTrac = false, bool setupIncl = false) where T : BaseEntity
        {
            var logger = Mock.Of<ILogger>();
            var dc = new DataContext("test", logger, test: true, testName: $"FilterByIsDeleted{name}");
            var noDeletesDc = new AllDataContext(test: true, testName: $"Unfiltered{name}");

            var repoM = new Mock<Repository>(dc, noDeletesDc);
            if (setupNoTrac)
                repoM.Setup(m => m.NoTracking(It.IsAny<DbSet<T>>()))
                    .Returns(dc.ISet<T>())
                    .Verifiable();
            if (setupIncl)
                repoM.Setup(m => m.Include(It.IsAny<IQueryable<T>>(), It.IsAny<Expression<Func<T, object>>>()))
                    .Returns(dc.ISet<T>())
                    .Verifiable();
            return repoM;
        }

        [TestMethod, TestCategory("Repository")]
        public void Page_SortsAsRequested()
        {
            // Setup
            var corresp = new List<User>()
            {
                new User() {Created = new DateTime(2016, 3, 14)},
                new User() {Created = new DateTime(2013, 3, 14)},
                new User() {Created = new DateTime(2011, 3, 14)},
            };
            var repo = GetRepoWithData(corresp, nameof(Page_SortsAsRequested));
            var a = new Dictionary<string, dynamic>()
            {
                {"Created", (Expression<Func<User, DateTime>>)(t => t.Created)}
            };
            var f = new SortFactory<User>(new[] { new SortSpecification("Created", SortDirection.Ascending) }, a);

            // Test
            var results = repo.Page(Specification<User>.All(), 0, 3, f);

            // Assert
            Assert.IsTrue(corresp.OrderBy(c => c.Created).SequenceEqual(results, new EqComparer<User>()));
        }

        [TestMethod, TestCategory("Repository")]
        public void Page_SortsDescAsRequested()
        {
            var r = new List<Role>()
            {
                new Role() {Created = new DateTime(2011, 3, 14)},
                new Role() {Created = new DateTime(2013, 3, 14)},
                new Role() {Created = new DateTime(2016, 3, 14)},
            };
            var repo = GetRepoWithData(r, nameof(Page_SortsDescAsRequested));
            var a = new Dictionary<string, dynamic>()
            {
                { "Created", (Expression<Func<Role, DateTime>>)(t => t.Created) }
            };
            var f = new SortFactory<Role>(new[] { new SortSpecification("Created", SortDirection.Descending) }, a);

            var results = repo.Page(Specification<Role>.All(), 0, 3, f);

            Assert.IsTrue(r.OrderByDescending(c => c.Created).SequenceEqual(results, new EqComparer<Role>()));
        }

        [TestMethod, TestCategory("Repository")]
        public void Page_Pages()
        {
            var r = new List<User>()
            {
                new User() { Id = 1},
                new User() { Id = 2},
                new User() { Id = 3},
                new User() { Id = 4},
                new User() { Id = 5},
            };
            var repo = GetRepoWithData(r, nameof(Page_Pages));

            var results = repo.Page(Specification<User>.All(), 1, 2);

            Assert.IsTrue(results.First().Id == 3);
            Assert.IsTrue(results.ElementAt(1).Id == 4);
        }

        [TestMethod, TestCategory("Repository")]
        public void Page_Filters()
        {
            var r = new List<User>()
            {
                new User() { Id = 1},
                new User() { Id = 2},
                new User() { Id = 3},
                new User() { Id = 4},
                new User() { Id = 5},
            };
            var repo = GetRepoWithData(r, nameof(Page_Filters));

            var results = repo.Page(Specification<User>.Start(c => c.Id > 3), 0, 2);

            Assert.IsTrue(results.First().Id == 4);
            Assert.IsTrue(results.ElementAt(1).Id == 5);
        }

        [TestMethod, TestCategory("Repository")]
        public void Page_NoTrackingDefault()
        {
            var repoM = GetRepoMock<Role>(nameof(Page_NoTrackingDefault));
            repoM.Object.Page(Specification<Role>.All());
            repoM.Verify();
        }

        [TestMethod, TestCategory("Repository")]
        public void Page_TrackingIfSpecified()
        {
            var repoM = GetRepoMock<Role>(nameof(Page_TrackingIfSpecified));
            repoM.Object.Page<Role>(Specification<Role>.All(), track: true);
            repoM.Verify(m => m.NoTracking(It.IsAny<DbSet<Role>>()), Times.Never);
        }

        [TestMethod, TestCategory("Repository")]
        public void Page_Includes()
        {
            var repoM = GetRepoMock<Role>(nameof(Page_Includes));
            repoM.Object.Page<Role>(Specification<Role>.All(), includes: r => r.UserRoles);
            repoM.Verify();
        }

        [TestMethod, TestCategory("Repository")]
        public void FindOne_TrackingIfSpecified()
        {
            var repoM = GetRepoMock<Role>(nameof(FindOne_TrackingIfSpecified));
            repoM.Object.FindOne<Role>(Specification<Role>.Start(c => c.Id > 3), track: true);
            repoM.Verify(m => m.NoTracking(It.IsAny<DbSet<Role>>()), Times.Never);
        }

        [TestMethod, TestCategory("Repository")]
        public void FindOne_NoTrackingDefault()
        {
            var repoM = GetRepoMock<Role>(nameof(FindOne_NoTrackingDefault), setupNoTrac: true);
            repoM.Object.FindOne<Role>(Specification<Role>.Start(c => c.Id > 3));
            repoM.Verify();
        }

        [TestMethod, TestCategory("Repository")]
        public void FindOne_Includes()
        {
            var repoM = GetRepoMock<Role>(nameof(FindOne_Includes), setupIncl: true);
            repoM.Object.FindOne<Role>(Specification<Role>.All(), includes: r => r.UserRoles);
            repoM.Verify();
        }

        [TestMethod, TestCategory("Repository")]
        public void FindOne_Filters()
        {
            var r = new List<User>()
            {
                new User() { Id = 1},
                new User() { Id = 2},
                new User() { Id = 3},
                new User() { Id = 4},
                new User() { Id = 5},
            };
            var repo = GetRepoWithData(r, nameof(FindOne_Filters));

            var result = repo.FindOne<User>(Specification<User>.Start(c => c.Id == 3));

            Assert.AreEqual(result.Id, 3);
        }

        [TestMethod, TestCategory("Repository")]
        public void FindAll_ExcludesSoftDeleted()
        {
            // Awaiting EFCore 2.0 for reimplementation (life-cycle hooks and global filters).
            /*var r = new List<User>()
            {
                new User() { Id = 1},
                new User() { Id = 2},
                new User() { Id = 3, IsDeleted = true },
                new User() { Id = 4},
                new User() { Id = 5, IsDeleted = true },
            };
            var repo = GetRepoWithData(r, nameof(FindOne_Filters));

            var results = repo.FindAll<User>(Specification<User>.Start(c => c.Id > 2), incSoftDel:false);

            Assert.AreEqual(1, results.First().Id); */
        }

        [TestMethod, TestCategory("Repository")]
        public void FindAll_TrackingIfSpecified()
        {
            var repoM = GetRepoMock<Role>(nameof(FindAll_TrackingIfSpecified), setupNoTrac: true);
            repoM.Object.FindAll<Role>(Specification<Role>.Start(c => c.Id > 3), track: true);
            repoM.Verify(m => m.NoTracking(It.IsAny<DbSet<Role>>()), Times.Never);
        }

        [TestMethod, TestCategory("Repository")]
        public void FindAll_NoTrackingDefault()
        {
            var repoM = GetRepoMock<Role>(nameof(FindAll_NoTrackingDefault), setupNoTrac: true);
            repoM.Object.FindAll<Role>(Specification<Role>.Start(c => c.Id > 3));
            repoM.Verify();
        }

        [TestMethod, TestCategory("Repository")]
        public void FindAll_Includes()
        {
            var repoM = GetRepoMock<Role>(nameof(FindAll_Includes), setupIncl: true);
            repoM.Object.FindAll<Role>(Specification<Role>.All(), includes: r => r.UserRoles);
            repoM.Verify();
        }

        [TestMethod, TestCategory("Repository")]
        public void FindAll_Filters()
        {
            var r = new List<User>()
            {
                new User() { Id = 1},
                new User() { Id = 2},
                new User() { Id = 3},
                new User() { Id = 4},
                new User() { Id = 5},
            };
            var repo = GetRepoWithData(r, nameof(FindAll_Filters));

            var results = repo.FindAll<User>(Specification<User>.Start(c => c.Id > 3));

            Assert.AreEqual(results.First().Id, 4);
            Assert.AreEqual(results.Skip(1).First().Id, 5);
        }

        [TestMethod, TestCategory("Repository")]
        public void Delete_NullEntityIgnored()
        {
            var r = new List<User>()
            {
                new User() { Id = 1},
                new User() { Id = 2},
                new User() { Id = 3},
                new User() { Id = 4},
                new User() { Id = 5},
            };
            var repo = GetRepoWithData(r, nameof(Delete_NullEntityIgnored));

            var result = repo.Delete<User>(Specification<User>.Start(c => c.Id == 7));

            Assert.IsNull(result);
        }

        [TestMethod, TestCategory("Repository")]
        public void Delete_RemovesFromSet()
        {
            var r = new List<User>()
            {
                new User() { Id = 1},
                new User() { Id = 2},
                new User() { Id = 3},
                new User() { Id = 4},
                new User() { Id = 5},
            };
            var repo = GetRepoWithData(r, nameof(Delete_RemovesFromSet));

            var result = repo.Delete<User>(Specification<User>.Start(c => c.Id == 3));

            Assert.AreEqual(3, result.Id);
        }
    }
}
