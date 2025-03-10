using CardPayment.Domain.Models;
using CardPaymentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CardPayment.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Card> Cards { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<TransactionFee> TransactionFees { get; set; }
        DbSet<CardAuthorizationLog> CardAuthorizationLogs { get; set; }
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
