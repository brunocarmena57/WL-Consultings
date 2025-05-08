namespace WLConsultingChallenge.Core.DTOs;

public class TransactionDto
{
    public int Id { get; set; }
    public string FromUsername { get; set; }
    public string ToUsername { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
}