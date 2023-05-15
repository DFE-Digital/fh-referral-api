namespace FamilyHubs.Referral.Data.Entities;

//New, Opened, Accepted, Declined in this sort order 

public class Status : EntityBase<long>
{
    public required string Name { get; set; }
    public required byte SortOrder { get; set; }
}
