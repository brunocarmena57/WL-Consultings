using WLConsultingChallenge.Core.Entities;
using WLConsultingChallenge.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace WLConsultingChallenge.Core.Services;

public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;

        public WalletService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> GetWalletByUserId(int userId)
        {
            var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                throw new Exception("Wallet not found.");

            return (Wallet)wallet;
        }

        public async Task<decimal> GetBalance(int userId)
        {
            var wallet = await GetWalletByUserId(userId);
            return wallet?.Balance ?? 0;
        }

        public async Task<Transaction> Deposit(int userId, decimal amount, string description)
        {
            var wallet = await GetWalletByUserId(userId);
            if (wallet == null)
                throw new Exception("Wallet not found");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    wallet.Balance += amount;
                    wallet.UpdatedAt = DateTime.UtcNow;

                    var transactionRecord = new Transaction
                    {
                        ToWalletId = wallet.Id,
                        Amount = amount,
                        Type = TransactionType.Deposit,
                        CreatedAt = DateTime.UtcNow,
                        Description = description
                    };

                    _context.Transactions.Add(transactionRecord);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return transactionRecord;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<Transaction> Transfer(int fromUserId, int toUserId, decimal amount, string description)
        {
            var fromWallet = await GetWalletByUserId(fromUserId);
            var toWallet = await GetWalletByUserId(toUserId);

            if (fromWallet == null || toWallet == null)
                throw new Exception("One or both wallets not found");

            if (fromWallet.Balance < amount)
                throw new Exception("Insufficient funds");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    fromWallet.Balance -= amount;
                    toWallet.Balance += amount;
                    
                    fromWallet.UpdatedAt = DateTime.UtcNow;
                    toWallet.UpdatedAt = DateTime.UtcNow;

                    var transactionRecord = new Transaction
                    {
                        FromWalletId = fromWallet.Id,
                        ToWalletId = toWallet.Id,
                        Amount = amount,
                        Type = TransactionType.Transfer,
                        CreatedAt = DateTime.UtcNow,
                        Description = description
                    };

                    _context.Transactions.Add(transactionRecord);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return transactionRecord;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserId(int userId, DateTime? startDate, DateTime? endDate)
        {
            var wallet = await GetWalletByUserId(userId);
            if (wallet == null)
                throw new Exception("Wallet not found");

            var query = _context.Transactions
                .Include(t => t.FromWallet).ThenInclude(w => w.User)
                .Include(t => t.ToWallet).ThenInclude(w => w.User)
                .Where(t => t.FromWalletId == wallet.Id || t.ToWalletId == wallet.Id);

            if (startDate.HasValue)
                query = query.Where(t => t.CreatedAt >= startDate);

            if (endDate.HasValue)
                query = query.Where(t => t.CreatedAt <= endDate);

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }
    }
}