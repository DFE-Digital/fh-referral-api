namespace FamilyHubs.ReferralCommon.Shared.Dto;

//Will be moved into a seperate NuGet Package

public record ReferrerDto : DtoBase<long>
{
    public required string EmailAddress { get; set; }

    public override int GetHashCode()
    {
        return
           EqualityComparer<string>.Default.GetHashCode(EmailAddress) * -1521134295;
    }

    public virtual bool Equals(ReferrerDto? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other))
            return true;

        return
            EqualityComparer<string>.Default.Equals(EmailAddress, other.EmailAddress);
    }
}
