using JorgeCred.Domain;
using Microsoft.EntityFrameworkCore;

namespace JorgeCred.Data.Context
{
    public class JorgeContext : DbContext
    {
        public JorgeContext(DbContextOptions<JorgeContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.OriginAccount)
                .WithMany()
                .HasForeignKey(t => t.OriginAccountId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.TargetAccount)
                .WithMany()
                .HasForeignKey(t => t.TargetAccountId);
        }

        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
