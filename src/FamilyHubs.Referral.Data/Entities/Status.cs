namespace FamilyHubs.Referral.Data.Entities;

public class Status : EntityBase<byte>
{
    public required string Name { get; set; } = default!;
    public required byte SortOrder { get; set; }
    public required byte SecondrySortOrder { get; set; }

}
