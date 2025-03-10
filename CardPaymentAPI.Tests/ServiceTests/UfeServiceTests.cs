using CardPayment.Application.Common.Interfaces;
using CardPayment.Infrastructure.Data;
using CardPayment.Infrastructure.Services.Ufe;
using CardPaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CardPaymentAPI.Tests.ServiceTests
{
    public class UfeServiceTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _context;

        public UfeServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new AppDbContext(options);
            _serviceProvider = new ServiceCollection()
                .AddScoped<IAppDbContext>(_ => _context)
                .AddScoped<IUfeService, UfeService>()
                .BuildServiceProvider();
        }

        [Fact]
        public async Task Should_Load_Initial_Fee()
        {
            // Arrange
            var fee = new TransactionFee { Amount = 1.0m, UpdatedAt = DateTime.UtcNow.AddMinutes(-10) };
            _context.TransactionFees.Add(fee);
            await _context.SaveChangesAsync();

            var ufeService = _serviceProvider.GetRequiredService<IUfeService>();

            // Act
            var currentFee = ufeService.GetCurrentFee();

            // Assert
            Assert.Equal(1.0m, currentFee);
        }

        [Fact]
        public async Task Should_Update_Fee_Over_Time()
        {
            // Arrange
            var fee = new TransactionFee { Amount = 1.0m, UpdatedAt = DateTime.UtcNow.AddMinutes(-10) };
            _context.TransactionFees.Add(fee);
            await _context.SaveChangesAsync();

            var ufeService = _serviceProvider.GetRequiredService<IUfeService>();

            // Act
            var initialFee = ufeService.GetCurrentFee();

            // Emulate the passing of time by triggering the fee update manually.
            var updateMethod = typeof(UfeService).GetMethod("UpdateFee", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updateMethod?.Invoke(ufeService, new object[] { null });

            // Give some time for the fee to be updated
            await Task.Delay(1000);

            var updatedFee = ufeService.GetCurrentFee();

            // Assert that the fee has changed after the update
            Assert.NotEqual(initialFee, updatedFee);
        }

        [Fact]
        public async Task Should_Not_Update_Fee_If_No_TransactionFee_Exists()
        {
            // Arrange: no fees in the database
            var ufeService = _serviceProvider.GetRequiredService<IUfeService>();

            // Act
            var currentFee = ufeService.GetCurrentFee();

            // Assert: default fee should be 1.0 if no fees exist
            Assert.Equal(1.0m, currentFee);
        }
    }
}
