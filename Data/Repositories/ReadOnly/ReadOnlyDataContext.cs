using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.ReadOnly
{
    public class ReadOnlyDataContext : DbContext, IReadOnlyDataContext
    {
        private bool Test;
        private string TestName;

        public ReadOnlyDataContext(bool test = false, string testName = "")
        {
            this.Test = test;
            this.TestName = testName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder opts)
        {
            if (this.Test)
                opts.UseInMemoryDatabase(TestName);
        }

        public DbSet<V_Population> Population { get; set; }

        public DbSet<T> ISet<T>() where T : class => Set<T>();
    }
}
