namespace FamilyHubs.Referral.Data.Entities;

public class ReferralStatus : EntityBase<byte>
{
    public required string Name { get; set; } = default!;
    public required byte SortOrder { get; set; }
    public required byte SecondrySortOrder { get; set; }

}
