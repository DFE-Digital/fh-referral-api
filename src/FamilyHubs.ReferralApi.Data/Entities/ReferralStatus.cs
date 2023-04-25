namespace FamilyHubs.ReferralApi.Data.Entities;

public class ReferralStatus : EntityBase<long>
{
    public required string Status { get; set; } = default!;
    public required long ReferralId { get; set; } = default!;

}
