using WLConsultingChallenge.Core.Entities;

namespace WLConsultingChallenge.Core.Services;

public interface IWalletService
{
    Task<Wallet> GetWalletByUserId(int userId);
    Task<decimal> GetBalance(int userId);
    Task<Transaction> Deposit(int userId, decimal amount, string description);
    Task<Transaction> Transfer(int fromUserId, int toUserId, decimal amount, string description);
    Task<IEnumerable<Transaction>> GetTransactionsByUserId(int userId, DateTime? startDate, DateTime? endDate);
}