namespace WLConsultingChallenge.Core.DTOs;

public class CreateTransactionDto
{
    public int ToUserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}