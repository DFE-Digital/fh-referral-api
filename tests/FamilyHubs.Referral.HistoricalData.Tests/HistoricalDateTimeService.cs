using FamilyHubs.Referral.Data.Interceptors;

namespace FamilyHubs.Referral.HistoricalData.Tests;

public class HistoricalDateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow.AddYears(-8);
}

