using CardPayment.Application.Common.Interfaces;
using CardPaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CardPayment.Infrastructure.Services.Ufe;

public class UfeBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private decimal _currentFee;

    public UfeBackgroundService(IServiceProvider serviceProvider, IDateTimeProvider dateTimeProvider)
    {
        _serviceProvider = serviceProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _currentFee = await LoadLatestFee();

        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateFeeAsync();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task<decimal> LoadLatestFee()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var lastFee = await context.TransactionFees
                .OrderByDescending(f => f.UpdatedAt)
                .FirstOrDefaultAsync();

            return lastFee?.Amount ?? 1.0m;
        }
    }

    private async Task UpdateFeeAsync()
    {
        var random = new Random();
        var multiplier = (decimal)random.NextDouble() * 2;
        _currentFee *= multiplier;

        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            if (context != null)
            {
                var newFee = TransactionFee.CreateTransactionFee(_currentFee, _dateTimeProvider.UtcNow);
                context.TransactionFees.Add(newFee);
                await context.SaveChangesAsync();
            }
        }
    }
}
