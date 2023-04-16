using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LocationFindingService.Models.Data
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<IPAddress> IPAddresses { get; set; }
        public string DbPath { get; }

        public DatabaseContext() { }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            string workingDirectory = Environment.CurrentDirectory;
            DbPath = Path.Join(workingDirectory, """\crezcotest.db""");

            if (!File.Exists(DbPath))
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IPAddress>()
                .HasIndex(u => u.IpAddress)
                .IsUnique();
        }
    }

    public class InMemoryDatabaseContext : DatabaseContext
    {
        public InMemoryDatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
        }
    }
}
