using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CardPayment.Domain.Models;

namespace CardPaymentAPI.Models;

public class CardAuthorizationLog
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("CardId")]
    public required Card Card { get; set; }

    public bool IsAuthorized { get; set; }

    public string? Reason { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static CardAuthorizationLog CreateCardAuthorizationLog(Card card, bool isAuthorized, string reason)
    {
        return new CardAuthorizationLog
        {
            Card = card,
            IsAuthorized = isAuthorized,
            Reason = reason
        };
    }
}
