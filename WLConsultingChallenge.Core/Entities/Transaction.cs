namespace WLConsultingChallenge.Core.Entities;

public class Transaction
{
    public int Id { get; set; }
    public int? FromWalletId { get; set; }
    public int ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public virtual Wallet FromWallet { get; set; }
    public virtual Wallet ToWallet { get; set; }
}