using CardPayment.Application.Common.Interfaces;
using CardPayment.Infrastructure.Services.Ufe;
using CardPaymentAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CardPayment.Application.Transactions.Commands
{
    public class PayWithCardCommand : IRequest<PayWithCardResult>
    {
        public int CardId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PayWithCardResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public decimal Fee { get; set; }
    }

    public class PayWithCardHandler : IRequestHandler<PayWithCardCommand, PayWithCardResult>
    {
        private readonly IAppDbContext _context;
        private readonly IUfeService _ufeService;

        public PayWithCardHandler(IAppDbContext context, IUfeService ufeService)
        {
            _context = context;
            _ufeService = ufeService;
        }

        public async Task<PayWithCardResult> Handle(PayWithCardCommand request, CancellationToken cancellationToken)
        {
            var card = await _context.Cards.Include(x => x.Transactions).FirstOrDefaultAsync(x => x.Id == request.CardId);
            if (card == null) return new PayWithCardResult { IsSuccess = false, ErrorMessage = "Card not found" };

            var transactionFee = _ufeService.GetCurrentFee();
            var totalAmount = request.Amount + transactionFee;

            var isAuthorized = await card.IsCardAuthorized(totalAmount);
            if (!isAuthorized)
                return new PayWithCardResult { IsSuccess = false, ErrorMessage = "Card is not authorized" };

            card.UpdateBalance(-totalAmount);
            _context.Transactions.Add(Transaction.CreateNewTransaction(card, request.Amount, _ufeService.GetCurrentFee(), DateTime.UtcNow));

            await _context.SaveChangesAsync();
            return new PayWithCardResult { IsSuccess = true, Fee = transactionFee };
        }
    }
}
