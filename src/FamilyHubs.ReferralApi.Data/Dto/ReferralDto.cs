namespace FamilyHubs.ReferralCommon.Shared.Dto;

//Will be moved into a seperate NuGet Package

public record ReferralDto : DtoBase<long>
{
    public required string ReasonForSupport { get; set; }
    public required string EngageWithFamily { get; set; }
    public required RecipientDto RecipientDto { get; set; }
    public required ReferrerDto ReferrerDto { get; set; }
    public required ReferralServiceDto ReferralServiceDto { get; set; }
    public ICollection<ReferralStatusDto> Status { get; set; } = new List<ReferralStatusDto>();

    public override int GetHashCode()
    {
        return
           EqualityComparer<string?>.Default.GetHashCode(ReasonForSupport) * -1521134295 +
           EqualityComparer<string?>.Default.GetHashCode(EngageWithFamily) * -1521134295;
    }

    public virtual bool Equals(ReferralDto? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other))
            return true;

        return
            EqualityComparer<string>.Default.Equals(ReasonForSupport, other.ReasonForSupport) &&
            EqualityComparer<string>.Default.Equals(EngageWithFamily, other.EngageWithFamily);
    }
}
