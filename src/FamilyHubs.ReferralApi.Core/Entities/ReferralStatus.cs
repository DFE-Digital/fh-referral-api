using FamilyHubs.SharedKernel;

namespace FamilyHubs.ReferralApi.Core.Entities;

public class ReferralStatus : EntityBase<long>
{
    public required string Status { get; set; } = default!;
    public required long ReferralId { get; set; } = default!;

}
