using Data.Utilities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using Data.QueryableExtensions;
using Xunit;

namespace Data.Tests
{
    public class RepositoryTest
    {
        public RepositoryTest() // reset anything static to a known state before each test
        {
            // Extension method overrides
            QueryableExtensions.QueryableExtensions.Includer = new NullIncluder();
            QueryableExtensions.QueryableExtensions.FirstOrDefaulter = new DbFirstOrDefaulter();
        }

        protected Repository GetRepoWithData<T>(List<T> data, string name) where T : BaseEntity
        {
            var logger = Mock.Of<ILogger>();
            var tu = Mock.Of<IPrincipal>();
            var dc = new DataContext(tu, logger, test: true, testName: $"FilterByIsDeleted{name}");
            var noDeletesDc = new AllDataContext(test: true, testName: $"Unfiltered{name}");

            data.ForEach(c => dc.ISet<T>().Add(c));
            data.ForEach(c => { if (!c.IsDeleted) noDeletesDc.ISet<T>().Add(c); });
            dc.SaveChanges();
            noDeletesDc.SaveChanges();

            var repo = new Repository(dc, noDeletesDc);
            return repo;
        }

        protected Mock<Repository> GetRepoMock<T>(string name, bool setupNoTrac = false) where T : BaseEntity
        {
            var logger = Mock.Of<ILogger>();
            var tu = Mock.Of<IPrincipal>();
            var dc = new DataContext(tu, logger, test: true, testName: $"FilterByIsDeleted{name}");
            var noDeletesDc = new AllDataContext(test: true, testName: $"Unfiltered{name}");

            var repoM = new Mock<Repository>(dc, noDeletesDc);
            if (setupNoTrac)
                repoM.Setup(m => m.NoTracking(It.IsAny<DbSet<T>>()))
                    .Returns(dc.ISet<T>())
                    .Verifiable();
            return repoM;
        }

        [Fact]
        public void AddEntity_AddsToUnderlyingContext()
        {
            var ds = Mock.Of<DbSet<User>>();
            var dcm = new Mock<DataContext>(Mock.Of<IPrincipal>(), Mock.Of<ILogger>(), false, "");
            dcm.Setup(d => d.ISet<User>()).Returns(ds);
            var repo = new Repository(dcm.Object, Mock.Of<ISoftDeletedDataContext>());
            var user = new User();

            repo.AddEntity(user);

            Mock.Get(ds).Verify(d => d.Add(It.Is<User>(u => u == user)));
        }

        [Fact]
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
            var f = new SortFactory<User, long>(new[] { new SortSpecification("Created", SortDirection.Ascending) }, a);

            // Test
            var results = repo.Page(Specification<User>.All(), 0, 3, f);

            // Assert
            Assert.True(corresp.OrderBy(c => c.Created).SequenceEqual(results, new EqComparer<User>()));
        }

        [Fact]
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
            var f = new SortFactory<Role, long>(new[] { new SortSpecification("Created", SortDirection.Descending) }, a);

            var results = repo.Page(Specification<Role>.All(), 0, 3, f);

            Assert.True(r.OrderByDescending(c => c.Created).SequenceEqual(results, new EqComparer<Role>()));
        }

        [Fact]
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

            Assert.True(results.First().Id == 3);
            Assert.True(results.ElementAt(1).Id == 4);
        }

        [Fact]
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

            Assert.True(results.First().Id == 4);
            Assert.True(results.ElementAt(1).Id == 5);
        }

        [Fact]
        public void Page_NoTrackingDefault()
        {
            var repoM = GetRepoMock<Role>(nameof(Page_NoTrackingDefault));
            repoM.Object.Page(Specification<Role>.All());
            repoM.Verify();
        }

        [Fact]
        public void Page_TrackingIfSpecified()
        {
            var repoM = GetRepoMock<Role>(nameof(Page_TrackingIfSpecified));
            repoM.Object.Page<Role>(Specification<Role>.All(), track: true);
            repoM.Verify(m => m.NoTracking(It.IsAny<DbSet<Role>>()), Times.Never);
        }

        [Fact]
        public void Page_Includes()
        {
            var repoM = GetRepoMock<Role>(nameof(Page_Includes));
            repoM.Object.Page<Role>(Specification<Role>.All(), includes: r => r.UserRoles);
            repoM.Verify();
        }

        [Fact]
        public void FindOne_TrackingIfSpecified()
        {
            var repoM = GetRepoMock<Role>(nameof(FindOne_TrackingIfSpecified));
            repoM.Object.FindOne<Role>(Specification<Role>.Start(c => c.Id > 3), track: true);
            repoM.Verify(m => m.NoTracking(It.IsAny<DbSet<Role>>()), Times.Never);
        }

        [Fact]
        public void FindOne_NoTrackingDefault()
        {
            var repoM = GetRepoMock<Role>(nameof(FindOne_NoTrackingDefault), setupNoTrac: true);
            repoM.Object.FindOne<Role>(Specification<Role>.Start(c => c.Id > 3));
            repoM.Verify();
        }

        [Fact]
        public void FindOne_Includes()
        {
            QueryableExtensions.QueryableExtensions.Includer = Mock.Of<QueryableExtensions.IIncluder>();
            var repoM = GetRepoMock<Role>(nameof(FindOne_Includes));
            repoM.Object.FindOne<Role>(Specification<Role>.All(), includes: r => r.UserRoles);
            Mock.Get(QueryableExtensions.QueryableExtensions.Includer)
                .Verify(i => i.Include(It.IsAny<IQueryable<Role>>(), It.IsAny<Expression<Func<Role, object>>>()), Times.Once);

        }

        [Fact]
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

            Assert.Equal(result.Id, 3);
        }

        [Fact]
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

            Assert.Equal(1, results.First().Id); */
        }

        [Fact]
        public void FindAll_TrackingIfSpecified()
        {
            var repoM = GetRepoMock<Role>(nameof(FindAll_TrackingIfSpecified), setupNoTrac: true);
            repoM.Object.FindAll<Role>(Specification<Role>.Start(c => c.Id > 3), track: true);
            repoM.Verify(m => m.NoTracking(It.IsAny<DbSet<Role>>()), Times.Never);
        }

        [Fact]
        public void FindAll_NoTrackingDefault()
        {
            var repoM = GetRepoMock<Role>(nameof(FindAll_NoTrackingDefault), setupNoTrac: true);
            repoM.Object.FindAll<Role>(Specification<Role>.Start(c => c.Id > 3));
            repoM.Verify();
        }

        [Fact]
        public void FindAll_Includes()
        {
            QueryableExtensions.QueryableExtensions.Includer = Mock.Of<QueryableExtensions.IIncluder>();
            var repoM = GetRepoMock<Role>(nameof(FindOne_Includes));
            repoM.Object.FindAll<Role>(Specification<Role>.All(), includes: r => r.UserRoles);
            Mock.Get(QueryableExtensions.QueryableExtensions.Includer)
                .Verify(i => i.Include(It.IsAny<IQueryable<Role>>(), It.IsAny<Expression<Func<Role, object>>>()), Times.Once);
        }

        [Fact]
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

            Assert.Equal(results.First().Id, 4);
            Assert.Equal(results.Skip(1).First().Id, 5);
        }

        [Fact]
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

            Assert.Null(result);
        }

        [Fact]
        public void Delete_Soft_SetsIsDeleted()
        {
            var r = new List<User>()
            {
                new User() { Id = 1},
                new User() { Id = 2},
                new User() { Id = 3},
                new User() { Id = 4},
                new User() { Id = 5},
            };
            var repo = GetRepoWithData(r, nameof(Delete_Soft_SetsIsDeleted));

            var result = repo.Delete<User>(Specification<User>.Start(c => c.Id == 3));

            Assert.Equal(3, result.Id);
            Assert.True(result.IsDeleted);
        }

        [Theory]
        [InlineData(true, 0, 1)]
        [InlineData(false, 1, 0)]
        public void Save_CallsAppropriateDCSave(Boolean allData, int allDataTimes, int excludeDelsTimes)
        {
            var dc = Mock.Of<IDataContext>(d => d.SaveChanges() == 0);
            var noDeletesDc = Mock.Of<ISoftDeletedDataContext>(d => d.SaveChanges() == 0);

            var repo = new Repository(dc, noDeletesDc);
            repo.Save(allData);

            Mock.Get(dc).Verify(d => d.Save(), Times.Exactly(allDataTimes));
            Mock.Get(noDeletesDc).Verify(d => d.Save(), Times.Exactly(excludeDelsTimes));
        }

        [Fact]
        public void Delete_CallsRemoveWhenHardDelete()
        {
            var model = new User();
            var df = Mock.Of<IFirstOrDefaulter>(i =>
                i.FirstOrDefault<User>(It.IsAny<IQueryable<User>>(),
                    It.IsAny<Expression<Func<User, bool>>>()) == model);
            var dbset = Mock.Of<DbSet<User>>();
            var dc = Mock.Of<IDataContext>(d => d.ISet<User>() == dbset);
            var noDeletesDc = Mock.Of<ISoftDeletedDataContext>(d => d.SaveChanges() == 0);
            var repo = new Repository(dc, noDeletesDc);
            QueryableExtensions.QueryableExtensions.FirstOrDefaulter = df;

            repo.Delete(Specification<User>.True, true);

            Mock.Get(dbset).Verify(d => d.Remove(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void Delete_IncludesAll()
        {
            var incm = new Mock<IIncluder>();
            incm.Setup(i => i.Include(It.IsAny<IQueryable<User>>(), It.IsAny<Expression<Func<User, object>>>()))
                .Returns((IQueryable<User> a, Expression<Func<User, object>> expr) => a);
            var df = Mock.Of<IFirstOrDefaulter>();
            var dbset = Mock.Of<DbSet<User>>();
            var dc = Mock.Of<IDataContext>(d => d.ISet<User>() == dbset);
            var noDeletesDc = Mock.Of<ISoftDeletedDataContext>(d => d.SaveChanges() == 0);
            var repo = new Repository(dc, noDeletesDc);
            QueryableExtensions.QueryableExtensions.Includer = incm.Object;
            QueryableExtensions.QueryableExtensions.FirstOrDefaulter = df;

            repo.Delete(Specification<User>.True, false, u => u.UserRoles, u => u.UserRoles.Select(ur => ur.User));

            incm.Verify(i => i.Include(It.IsAny<IQueryable<User>>(), It.IsAny<Expression<Func<User, object>>>()), Times.Exactly(2));
        }
    }
}
