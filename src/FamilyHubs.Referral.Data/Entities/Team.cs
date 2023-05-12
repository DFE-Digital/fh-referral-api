namespace FamilyHubs.Referral.Data.Entities;


public class Team : EntityBase<long>
{
    public required long OrganisationId { get; set; }
    public required long ReferrerId { get; set; }
    public string? Name { get; set; }
    
}