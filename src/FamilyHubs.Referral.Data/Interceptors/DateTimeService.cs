namespace FamilyHubs.Referral.Data.Interceptors;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}
