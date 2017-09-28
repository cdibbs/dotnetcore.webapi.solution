using System;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Data.Repositories.ReadOnly
{
    public class ReadOnlyDataContext : DbContext, IReadOnlyDataContext
    {
        private bool Test;
        private string TestName;
        private string ConnectionString;

        public ReadOnlyDataContext(string connectionString, bool test = false, string testName = "")
        {
            this.ConnectionString = connectionString;
            this.Test = test;
            this.TestName = testName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder opts)
        {
            if (this.Test)
                UseInMemoryDatabase(opts, TestName, null);
            else
            {
                UseSqlServer(opts, this.ConnectionString, null);
            }
        }

        public DbSet<V_MyView> V_NullMembershipEligibilityMappings { get; set; }

        public DbSet<T> ISet<T>() where T : class => Set<T>();

        // Extension methods need to go away...
        public Func<DbContextOptionsBuilder, string, Action<SqlServerDbContextOptionsBuilder>, DbContextOptionsBuilder> UseSqlServer =
            Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer;

        public Func<DbContextOptionsBuilder, string, Action<InMemoryDbContextOptionsBuilder>, DbContextOptionsBuilder> UseInMemoryDatabase =
            Microsoft.EntityFrameworkCore.InMemoryDbContextOptionsExtensions.UseInMemoryDatabase;
    }
}
