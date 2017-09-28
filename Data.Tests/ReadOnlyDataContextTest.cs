using System;
using System.Collections.Generic;
using System.Text;
using Data.Repositories.ReadOnly;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Xunit;

namespace Data.Tests
{
    public class ReadOnlyDataContextTest
    {
        [Fact]
        public void OnConfiguringUsesProdWhenNotUnderTest()
        {
            bool sqlcalled = false;
            var r = new ReadOnlyDataContextMockProxy("bogus", false);
            r.UseSqlServer = (a, b, c) =>
            {
                sqlcalled = true;
                return null;
            };

            r.TestOnConfiguring(Mock.Of<DbContextOptionsBuilder>());

            Assert.True(sqlcalled);
        }

        public class ReadOnlyDataContextMockProxy : ReadOnlyDataContext
        {
            public void TestOnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);
            }

            public ReadOnlyDataContextMockProxy(string connectionString, bool test = false, string testName = "") : base(connectionString, test, testName)
            {
            }
        }
    }
}
