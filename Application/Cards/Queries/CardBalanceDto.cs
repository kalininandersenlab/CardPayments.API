namespace CardPayment.Application.Cards.Queries;

public class CardBalanceDto
{
    public decimal Balance { get; set; }
    public decimal? AvailableCredit { get; set; }
}
