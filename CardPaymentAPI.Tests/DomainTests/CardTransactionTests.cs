using CardPayment.Domain.Models;
using CardPayment.Infrastructure.Data;
using CardPaymentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CardPayment.Tests.DomainTests
{
    public class CardTransactionTests
    {
        private readonly AppDbContext _context;

        public CardTransactionTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new AppDbContext(options);
        }

        [Fact]
        public async Task Should_Not_Authorize_When_Card_Is_Inactive()
        {
            var card = new Card { CardNumber = "1111222233334444", Id = 1, Balance = 500, IsActive = false };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            bool result = await card.IsCardAuthorized(100, DateTime.UtcNow);
            Assert.False(result);
        }

        [Fact]
        public async Task Should_Not_Authorize_When_Balance_Is_Low()
        {
            var card = new Card { CardNumber = "1111222233334444", Id = 2, Balance = 50, CreditLimit = 0, IsActive = true };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            bool result = await card.IsCardAuthorized(100, DateTime.UtcNow);
            Assert.False(result);
        }

        [Fact]
        public async Task Should_Authorize_When_Balance_Is_Sufficient()
        {
            var card = new Card { CardNumber = "1111222233334444", Id = 3, Balance = 200, CreditLimit = 100, IsActive = true };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            bool result = await card.IsCardAuthorized(100, DateTime.UtcNow);
            Assert.True(result);
        }

        [Fact]
        public async Task Should_Not_Authorize_Fraudulent_Transaction()
        {
            var card = new Card { CardNumber = "1111222233334444", Id = 4, Balance = 500, IsActive = true };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            _context.Transactions.Add(new Transaction { Card = card, Amount = 100, Timestamp = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            bool result = await card.IsCardAuthorized(100, DateTime.UtcNow);
            Assert.False(result);
        }
    }
}
