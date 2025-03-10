namespace CardPayment.Application.Transactions.Queries;

public class TransactionResponse
{
    public decimal Amount { get; set; }

    public decimal Fee { get; set; }

    public DateTime Timestamp { get; set; }

    public int CardId { get; set; }
}
