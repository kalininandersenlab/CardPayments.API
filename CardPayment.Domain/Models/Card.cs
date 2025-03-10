using CardPaymentAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CardPayment.Domain.Models;

public class Card
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(15)]
    public required string CardNumber { get; set; }

    [Required]
    public decimal Balance { get; set; }

    public decimal? CreditLimit { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    [ForeignKey("UserId")]
    public User User { get; set; }

    public List<Transaction> Transactions { get; set; } = new();
    public List<CardAuthorizationLog> CardAuthorizationLogs { get; set; } = new();

    public async Task<bool> IsCardAuthorized(decimal amount, DateTime dateTime)
    {
        if (!IsActive)
        {
            await LogAuthorization(this, false, "Card not found or inactive");
            return false;
        }

        decimal availableFunds = Balance + CreditLimit.GetValueOrDefault();
        if (availableFunds < amount)
        {
            await LogAuthorization(this, false, "Insufficient funds");
            return false;
        }

        bool isFraudulent = CheckFraudulentTransactions(amount, dateTime);
        if (isFraudulent)
        {
            await LogAuthorization(this, false, "Suspected fraud");
            return false;
        }

        await LogAuthorization(this, true, "Authorization successful");
        return true;
    }

    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
    }

    public void UpdateCard(decimal balance, decimal? creditLimit, bool isActive)
    {
        Balance = balance;
        CreditLimit = creditLimit;
        IsActive = isActive;
    }

    public async Task LogAuthorization(Card card, bool isAuthorized, string reason)
    {
        CardAuthorizationLogs.Add(CardAuthorizationLog.CreateCardAuthorizationLog(card, isAuthorized, reason));
    }

    public static Card CreateNewCard()
    {
        var random = new Random();

        return new Card
        {
            CardNumber = random.Next(100000000, 999999999).ToString() + random.Next(100000, 999999),
            Balance = random.Next(100, 1000),
            CreditLimit = random.Next(0, 2) == 1 ? random.Next(500, 2000) : null,
            IsActive = true,
        };
    }

    private bool CheckFraudulentTransactions(decimal amount, DateTime dateTime)
    {
        var recentTransactions = Transactions
            .Where(t => t.Card.Id == Id && t.Timestamp > dateTime.AddSeconds(-10));

        return recentTransactions.Any(t => t.Amount + t.Fee == Math.Round(amount, 2));
    }
}
