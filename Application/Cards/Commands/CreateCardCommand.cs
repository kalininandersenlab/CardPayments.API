using CardPayment.Application.Common.Interfaces;
using CardPayment.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CardPayment.Application.Cards.Commands;

public class CreateCardCommand : IRequest<Card>
{
}

public class CreateCardHandler : IRequestHandler<CreateCardCommand, Card>
{
    private readonly IAppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCardHandler(IAppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Card> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        var username = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users
            .Include(u => u.Cards)
            .FirstOrDefaultAsync(x => x.Username == username) ?? throw new KeyNotFoundException("User not found");

        var card = Card.CreateNewCard();
        user.Cards.Add(card);

        await _context.SaveChangesAsync();
        return card;
    }
}
