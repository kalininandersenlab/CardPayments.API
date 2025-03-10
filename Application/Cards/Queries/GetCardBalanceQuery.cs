using CardPayment.Application.Common.Interfaces;
using MediatR;

namespace CardPayment.Application.Cards.Queries;

public class GetCardBalanceQuery : IRequest<CardBalanceDto>
{
    public int CardId { get; }

    public GetCardBalanceQuery(int cardId)
    {
        CardId = cardId;
    }
}

public class GetCardBalanceHandler : IRequestHandler<GetCardBalanceQuery, CardBalanceDto>
{
    private readonly IAppDbContext _context;

    public GetCardBalanceHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CardBalanceDto> Handle(GetCardBalanceQuery request, CancellationToken cancellationToken)
    {
        var card = await _context.Cards.FindAsync(request.CardId);
        return card == null
            ? throw new KeyNotFoundException("Card was not found")
            : new CardBalanceDto
        {
            Balance = card.Balance,
            AvailableCredit = card.CreditLimit
        };
    }
}
