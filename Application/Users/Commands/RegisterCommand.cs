using CardPayment.Application.Common.Interfaces;
using CardPayment.Application.Common.Services.Identity;
using CardPayment.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CardPayment.Application.Users.Commands
{
    public class RegisterCommand : IRequest<AuthResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public RegisterCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAppDbContext context, IConfiguration config, IAuthService authService)
        {
            _context = context;
            _config = config;
            _authService = authService;
        }

        public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (existingUser != null)
            {
                return new AuthResult { IsSuccess = false, Message = "User already exists" };
            }

            var user = User.CreateUser(request.Username, _authService.HashPassword(request.Password), _authService.GenerateRefreshToken(), DateTime.UtcNow.AddDays(7));

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var token = _authService.GenerateJwtToken(user.Username);

            return new AuthResult
            {
                IsSuccess = true,
                Token = token,
                RefreshToken = user.RefreshToken
            };
        }
    }
}
