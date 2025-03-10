using System.ComponentModel.DataAnnotations;

namespace CardPaymentAPI.Models;

public class TransactionFee
{
    [Key]
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public static TransactionFee CreateTransactionFee(decimal fee, DateTime updatedAt)
    {
        return new TransactionFee { Amount = fee, UpdatedAt = updatedAt };
    }
}
