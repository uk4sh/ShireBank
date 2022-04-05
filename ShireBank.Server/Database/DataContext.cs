using Microsoft.EntityFrameworkCore;
using ShireBank.Server.Models;

namespace ShireBank.Server.Database
{
    public class DataContext : DbContext
    {
        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Account>(e =>
            {
                e.Property(a => a.Id).ValueGeneratedOnAdd();
                e.Property(a => a.Version).IsRowVersion().HasDefaultValue(0);
                e.HasIndex(a => new { a.FirstName, a.LastName }).IsUnique();
            });

            builder.Entity<Transaction>(e =>
            {
                e.Property(e => e.Id).ValueGeneratedOnAdd();
                e.Property(t => t.Timestamp).HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%f', 'NOW')");
            });
        }
    }
}
