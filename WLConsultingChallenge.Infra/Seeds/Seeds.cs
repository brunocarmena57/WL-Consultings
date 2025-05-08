
using WLConsultingChallenge.Core.Entities;
using WLConsultingChallenge.Core.Services;
using WLConsultingChallenge.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WLConsultingChallenge.Infrastructure.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedData(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var authService = services.GetRequiredService<IAuthService>();
                
                await context.Database.MigrateAsync();
                await SeedUsers(context, authService);
                await SeedTransactions(context);
                
                Console.WriteLine("Database seeded successfully!");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private static async Task SeedUsers(AppDbContext context, IAuthService authService)
        {
            if (await context.Users.AnyAsync())
                return;

            // Create sample users
            var users = new List<User>
            {
                new User
                {
                    Username = "john_doe",
                    Email = "john@example.com",
                    PasswordHash = authService.HashPassword("Password123!"),
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "jane_smith",
                    Email = "jane@example.com",
                    PasswordHash = authService.HashPassword("Password123!"),
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "bob_johnson",
                    Email = "bob@example.com",
                    PasswordHash = authService.HashPassword("Password123!"),
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Create wallets for users
            var wallets = users.Select(user => new Wallet
            {
                UserId = user.Id,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await context.Wallets.AddRangeAsync(wallets);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTransactions(AppDbContext context)
        {
            if (await context.Transactions.AnyAsync())
                return;

            var users = await context.Users.Include(u => u.Wallet).ToListAsync();
            var random = new Random();

            // Add initial deposits for all users
            foreach (var user in users)
            {
                var depositAmount = random.Next(1000, 5000);
                
                // Create deposit transaction
                var depositTransaction = new Transaction
                {
                    ToWalletId = user.Wallet.Id,
                    Amount = depositAmount,
                    Type = TransactionType.Deposit,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    Description = "Initial deposit"
                };

                // Update wallet balance
                user.Wallet.Balance += depositAmount;
                user.Wallet.UpdatedAt = DateTime.UtcNow;
                
                await context.Transactions.AddAsync(depositTransaction);
            }

            await context.SaveChangesAsync();

            // Create some transfers between users
            for (int i = 0; i < 15; i++)
            {
                var fromUser = users[random.Next(users.Count)];
                var toUser = users.FirstOrDefault(u => u.Id != fromUser.Id);
                
                if (toUser == null || fromUser.Wallet.Balance <= 0)
                    continue;

                var transferAmount = Math.Round(fromUser.Wallet.Balance * (decimal)random.NextDouble() * 0.3M, 2);
                if (transferAmount <= 0)
                    continue;

                // Create transfer transaction
                var transferTransaction = new Transaction
                {
                    FromWalletId = fromUser.Wallet.Id,
                    ToWalletId = toUser.Wallet.Id,
                    Amount = transferAmount,
                    Type = TransactionType.Transfer,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 15)),
                    Description = $"Transfer from {fromUser.Username} to {toUser.Username}"
                };

                // Update wallet balances
                fromUser.Wallet.Balance -= transferAmount;
                toUser.Wallet.Balance += transferAmount;
                fromUser.Wallet.UpdatedAt = DateTime.UtcNow;
                toUser.Wallet.UpdatedAt = DateTime.UtcNow;
                
                await context.Transactions.AddAsync(transferTransaction);
            }

            await context.SaveChangesAsync();
        }
    }
}