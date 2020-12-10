using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models
{
    public class JournalDbContext : DbContext
    {
        public DbSet<UserRecord> Records { get; set; }

        public JournalDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
