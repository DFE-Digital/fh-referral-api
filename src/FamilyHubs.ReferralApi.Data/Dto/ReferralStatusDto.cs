namespace FamilyHubs.ReferralCommon.Shared.Dto;

//Will be moved into a seperate NuGet Package

public record ReferralStatusDto : DtoBase<long>
{
    public string Status { get; set; } = default!;

    public override int GetHashCode()
    {
        return
            EqualityComparer<string>.Default.GetHashCode(Status) * -1521134295;
    }

    public virtual bool Equals(ReferralStatusDto? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other))
            return true;

        return
            EqualityComparer<string>.Default.Equals(Status, other.Status);
    }
}
