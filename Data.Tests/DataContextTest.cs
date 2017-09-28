using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using Serilog;
using Xunit;

namespace Data.Tests
{
    public class DataContextTest
    {
        private IPrincipal user;
        private IIdentity ident;
        private ILogger logger;
        public DataContextTest()
        {
            ident = Mock.Of<IIdentity>(p => p.Name == "myuser");
            user = Mock.Of<IPrincipal>(p => p.Identity == ident);
            logger = Mock.Of<ILogger>();
        }

        [Fact]
        public void Save_Modified_UpdatesTimeUser()
        {
            // Mock
            var piDay2017 = new DateTime(2017, 3, 14);
            var dc = new DataContext(user, logger, true, nameof(Save_Modified_UpdatesTimeUser));
            dc.Users.Add(new User() {Username = ident.Name, Id = 314});
            dc.Roles.Add(new Role()
            {
                Created = piDay2017,
                LastUpdated = piDay2017,
                LastUpdatedBy = 1
            });
            dc.SaveChanges();
            var role = dc.Roles.First();
            role.Description = "something changed";

            // Test
            dc.Save();

            role = dc.Roles.First();
            Assert.Equal(piDay2017, role.Created);
            Assert.Equal(314, role.LastUpdatedBy);
            Assert.True(DateTime.Today < role.LastUpdated);
        }

        [Fact]
        public void Save_Added_UpdatesTimeUserCreated()
        {
            // Mock
            var dc = new DataContext(user, logger, true, nameof(Save_Added_UpdatesTimeUserCreated));
            dc.Users.Add(new User() { Username = ident.Name, Id = 314 });
            dc.SaveChanges();
            dc.Roles.Add(new Role()
            {
                Description = "original"
            });

            // Test
            dc.Save();

            var role = dc.Roles.First();
            Assert.True(DateTime.Today < role.Created);
            Assert.True(DateTime.Today < role.LastUpdated);
            Assert.Equal(role.Created, role.LastUpdated);
            Assert.Equal(314, role.LastUpdatedBy);
        }

        [Fact]
        public void Save_RetrievingCurrentUserError_TrapsLogs()
        {
            var dc = new DataContext(user, logger, true, nameof(Save_RetrievingCurrentUserError_TrapsLogs));
            dc.Users = null; // easiest way to get an error from that block

            dc.Save();

            Mock.Get(logger).Verify(l => l.Error(It.IsAny<Exception>(), It.Is<string>(s => s.StartsWith("Error loading user"))), Times.Once);
        }

        [Fact]
        public void Save_NonBaseEntity_NoErrors()
        {
            // Mock
            var dc = new BoogeymanDC(user, logger, true, nameof(Save_Added_UpdatesTimeUserCreated));
            dc.TestModels.Add(new TestModel()
            {
                Id = 1
            });

            // Test
            dc.Save();
        }

        public class BoogeymanDC : DataContext
        {
            public BoogeymanDC(IPrincipal user, ILogger logger, bool test = false, string testName = "") : base(user, logger, test, testName)
            {
            }

            public DbSet<TestModel> TestModels { get; set; }
        }
    }
}
