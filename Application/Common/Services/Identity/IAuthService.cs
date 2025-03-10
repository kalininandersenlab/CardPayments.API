namespace CardPayment.Application.Common.Services.Identity
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
        string GenerateJwtToken(string username);
        string GenerateRefreshToken();
    }
}
