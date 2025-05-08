using System.Transactions;
using Microsoft.EntityFrameworkCore;
using WLConsultingChallenge.Core.Entities;

namespace WLConsultingChallenge.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WLConsultingChallenge.Core.Entities.Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);

            // Wallet configuration
            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.TransactionsFrom)
                .WithOne(t => t.FromWallet)
                .HasForeignKey(t => t.FromWalletId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.TransactionsTo)
                .WithOne(t => t.ToWallet)
                .HasForeignKey(t => t.ToWalletId);

            // Transaction configuration
            modelBuilder.Entity<WLConsultingChallenge.Core.Entities.Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance)
                .HasColumnType("decimal(18,2)");
        }
    }
}