using CardPayment.Application.Common.Interfaces;
using CardPayment.Application.Common.Services.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CardPayment.Application.Users.Commands;

public class RefreshTokenCommand : IRequest<AuthResult>
{
    public string RefreshToken { get; set; }

    public RefreshTokenCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly IAppDbContext _context;
    private readonly IAuthService _authService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenCommandHandler(IAppDbContext context, IAuthService authService, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _authService = authService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken);
        if (user == null || user.IsRefreshTokenExpired(_dateTimeProvider.UtcNow))
        {
            return new AuthResult { IsSuccess = false, Message = "Incorrect refresh token" };
        }

        var token = _authService.GenerateJwtToken(user.Username);
        return new AuthResult { IsSuccess = true, Token = token, RefreshToken = user.RefreshToken };
    }
}
