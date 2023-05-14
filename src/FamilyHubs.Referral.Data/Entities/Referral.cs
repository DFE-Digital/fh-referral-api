namespace FamilyHubs.Referral.Data.Entities;

//[PrimaryKey(nameof(Service), nameof(Status), nameof(Recipient), nameof(Team), nameof(Referrer))]
public class Referral : EntityBase<long>
{
    public required string ReasonForSupport { get; set; }

    public required string EngageWithFamily { get; set; }

    public string? ReasonForDecliningSupport { get; set; }

    public virtual required Service Service { get; set; }

    public virtual required Status Status { get; set; }

    public virtual required Recipient Recipient { get; set; }

    public virtual required Team Team { get; set; }

    public virtual required User Referrer { get; set; }
}
