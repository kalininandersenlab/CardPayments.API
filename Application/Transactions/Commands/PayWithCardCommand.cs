using CardPayment.Application.Common.Interfaces;
using CardPaymentAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CardPayment.Application.Transactions.Commands;

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
    private readonly IDateTimeProvider _dateTimeProvider;

    public PayWithCardHandler(IAppDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PayWithCardResult> Handle(PayWithCardCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        try
        {
            var card = await _context.Cards.Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == request.CardId, cancellationToken);
            if (card == null)
                return new PayWithCardResult { IsSuccess = false, ErrorMessage = "Card not found" };

            var transactionFee = (await _context.TransactionFees
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync(cancellationToken))?.Amount ?? 0;

            var totalAmount = request.Amount + transactionFee;
            var isAuthorized = await card.IsCardAuthorized(totalAmount, _dateTimeProvider.UtcNow);

            if (!isAuthorized)
                return new PayWithCardResult { IsSuccess = false, ErrorMessage = "Card is not authorized" };

            card.UpdateBalance(-totalAmount);
            _context.Transactions.Add(Transaction.CreateNewTransaction(card, request.Amount, transactionFee, _dateTimeProvider.UtcNow));

            await _context.SaveChangesAsync(cancellationToken);
            await _context.CommitTransactionAsync(transaction, cancellationToken);

            return new PayWithCardResult { IsSuccess = true, Fee = transactionFee };
        }
        catch
        {
            await _context.RollbackTransactionAsync(transaction, cancellationToken);
            throw;
        }
    }

}
