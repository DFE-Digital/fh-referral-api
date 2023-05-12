namespace FamilyHubs.Referral.Data.Entities;

//New, Opened, Accepted, Declined in this sort order 

public class Status : EntityBase<long>
{
    public string Name { get; set; }
    public byte SortOrder { get; set; }
}
