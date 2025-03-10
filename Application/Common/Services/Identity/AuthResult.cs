namespace CardPayment.Application.Common.Services.Identity;

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
