using CardPayment.Application.Common.Interfaces;
using CardPayment.Domain.Models;
using CardPaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace CardPayment.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Card> Cards { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<TransactionFee> TransactionFees { get; set; }

    public DbSet<CardAuthorizationLog> CardAuthorizationLogs { get; set; }

    public DbSet<User> Users { get; set; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level, CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(level, cancellationToken);
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        await transaction.RollbackAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Card)
            .WithMany(c => c.Transactions)
            .HasForeignKey("CardId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CardAuthorizationLog>()
            .HasOne(t => t.Card)
            .WithMany(c => c.CardAuthorizationLogs)
            .HasForeignKey("CardId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Card>()
            .HasOne(c => c.User)
            .WithMany(u => u.Cards)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
