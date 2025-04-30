using Microsoft.EntityFrameworkCore;
using WalletApp.Domain.Entities;

namespace WalletApp.Infrastructure.Persistence
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

        public DbSet<Wallet> Wallets => Set<Wallet>();

        public DbSet<Movement> Movements => Set<Movement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.Movements)
                .WithOne(m => m.Wallet!)
                .HasForeignKey(m => m.WalletId);
        }
    }
}
