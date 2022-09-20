using FamilyHubs.SharedKernel.Interfaces;

namespace FamilyHubs.ReferralApi.Infrastructure.Service;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
