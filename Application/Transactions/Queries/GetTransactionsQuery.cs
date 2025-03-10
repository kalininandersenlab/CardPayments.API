using CardPayment.Application.Common.Interfaces;
using CardPaymentAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CardPayment.Application.Transactions.Queries
{
    public class GetTransactionsQuery : IRequest<List<TransactionResponse>>
    {
        public int CardId { get; }

        public GetTransactionsQuery(int cardId)
        {
            CardId = cardId;
        }
    }

    public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, List<TransactionResponse>>
    {
        private readonly IAppDbContext _context;

        public GetTransactionsHandler(IAppDbContext context)
        {
            _context = context;
        }

        public Task<List<TransactionResponse>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Transactions
                .Where(t => t.Card.Id == request.CardId).AsNoTracking()
                .Select(x => new TransactionResponse()
                {
                    Amount = x.Amount,
                    Fee = x.Fee,
                    CardId = x.Card.Id,
                    Timestamp = x.Timestamp
                })
                .ToList());
        }
    }
}
