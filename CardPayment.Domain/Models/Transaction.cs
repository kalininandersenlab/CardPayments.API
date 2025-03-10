using CardPayment.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardPaymentAPI.Models;

public class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public decimal Fee { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [ForeignKey("CardId")]
    public Card Card { get; set; }

    public static Transaction CreateNewTransaction(Card card, decimal amount, decimal fee, DateTime timestamp)
    {
        return new Transaction
        {
            Card = card,
            Amount = amount,
            Fee = fee,
            Timestamp = timestamp
        };
    }
}
