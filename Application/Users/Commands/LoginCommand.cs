using CardPayment.Application.Common.Interfaces;
using CardPayment.Application.Common.Services.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CardPayment.Application.Users.Commands
{
    public class LoginCommand : IRequest<AuthResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IAppDbContext _dbContext;
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAppDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (user == null || _authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return new AuthResult { IsSuccess = false, Message = "Incorrect combination of password and username" };
            }

            var token = _authService.GenerateJwtToken(user.Username);
            var refreshToken = _authService.GenerateRefreshToken();
            user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new AuthResult
            {
                IsSuccess = true,
                Token = token,
                RefreshToken = refreshToken
            };
        }
    }
}
