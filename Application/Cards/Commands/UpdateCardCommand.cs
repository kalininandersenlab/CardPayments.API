﻿using CardPayment.Application.Common.Interfaces;
using MediatR;
using System.Data;

namespace CardPayment.Application.Cards.Commands;

public class UpdateCardCommand : IRequest<bool>
{
    public int CardId { get; set; }

    public decimal Balance { get; set; }

    public decimal? CreditLimit { get; set; }

    public bool IsActive { get; set; }
}

public class UpdateCardHandler : IRequestHandler<UpdateCardCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateCardHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var card = await _context.Cards.FindAsync(request.CardId);
            if (card == null) return false;

            card.UpdateCard(request.Balance, request.CreditLimit, request.IsActive);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

}
