using CardPayment.Application.Common.Interfaces;
using CardPaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CardPayment.Infrastructure.Services.Ufe
{
    public class UfeService : IUfeService
    {
        private readonly IServiceProvider _serviceProvider;
        private decimal _currentFee;
        private Timer _timer;

        public UfeService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _currentFee = LoadLatestFee().Result;
            _timer = new Timer(UpdateFee, null, TimeSpan.Zero, TimeSpan.FromHours(1));
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

        private async Task UpdateFeeAsync(object state)
        {
            var random = new Random();
            var multiplier = (decimal)random.NextDouble() * 2;
            _currentFee *= multiplier;

            using (var scope = _serviceProvider.CreateScope())
            {
                var newFee = TransactionFee.CreateTransactionFee(_currentFee, DateTime.UtcNow);
                var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                if (context != null)
                {
                    context.TransactionFees.Add(newFee);
                    await context.SaveChangesAsync();
                }
            }
        }

        private void UpdateFee(object state)
        {
            Task.Run(() => UpdateFeeAsync(state));
        }

        public decimal GetCurrentFee() => _currentFee;
    }
}
