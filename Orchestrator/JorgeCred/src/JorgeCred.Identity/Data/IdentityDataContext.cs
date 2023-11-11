using JorgeCred.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JorgeCred.Identity.Data
{
    public class IdentityDataContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.OriginAccount)
                .WithMany()
                .HasForeignKey(t => t.OriginAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.TargetAccount)
                .WithMany()
                .HasForeignKey(t => t.TargetAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(t => t.Account)
                .WithOne(e => e.ApplicationUser)
                .HasForeignKey<Account>(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Account> Account { get; set; }
    }
}
