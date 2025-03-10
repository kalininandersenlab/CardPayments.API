using CardPayment.Application.Common.Interfaces;

namespace CardPayment.Application.Common.Services.DateTimeProvider;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
