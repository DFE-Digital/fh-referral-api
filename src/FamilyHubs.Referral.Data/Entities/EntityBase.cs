namespace FamilyHubs.Referral.Data.Entities;

/// <summary>
/// Base types for all Entities which track state using a given Id.
/// </summary>
public abstract class EntityBase<TId>
{
    public TId Id { get; set; } = default!;

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
