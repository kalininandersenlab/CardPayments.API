using System.ComponentModel.DataAnnotations;

namespace CardPayment.Domain.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }

    public List<Card> Cards { get; set; } = new List<Card>();

    public static User CreateUser(string username, string password, string refreshToken, DateTime refreshTokenExpiration)
    {
        return new User
        {
            Username = username,
            PasswordHash = password,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = refreshTokenExpiration
        };
    }

    public void UpdateRefreshToken(string  refreshToken, DateTime refreshTokenExpiration)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiration = refreshTokenExpiration;
    }

    public bool IsRefreshTokenExpired(DateTime dateTime)
    {
        return RefreshTokenExpiration < dateTime;
    }
}
