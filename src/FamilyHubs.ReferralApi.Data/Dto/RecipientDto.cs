// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable CS8604

//Will be moved into a seperate NuGet Package

namespace FamilyHubs.ReferralCommon.Shared.Dto;

public record RecipientDto : DtoBase<long>
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? TextPhone { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? TownOrCity { get; set; }
    public string? Country { get; set; }
    public string? PostCode { get; set; }

    public override int GetHashCode()
    {
        return
            EqualityComparer<string>.Default.GetHashCode(Name) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(Email) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(Telephone) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(TextPhone) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(AddressLine1) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(AddressLine2) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(TownOrCity) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(Country) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(PostCode) * -1521134295
            ;
    }


    public virtual bool Equals(RecipientDto? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other))
            return true;

        return
            EqualityComparer<string>.Default.Equals(Name, other.Name) &&
            EqualityComparer<string?>.Default.Equals(Email, other.Email) &&
            EqualityComparer<string?>.Default.Equals(Telephone, other.Telephone) &&
            EqualityComparer<string?>.Default.Equals(TextPhone, other.TextPhone) &&
            EqualityComparer<string?>.Default.Equals(AddressLine1, other.AddressLine1) &&
            EqualityComparer<string?>.Default.Equals(AddressLine2, other.AddressLine2) &&
            EqualityComparer<string?>.Default.Equals(TownOrCity, other.TownOrCity) &&
            EqualityComparer<string?>.Default.Equals(Country, other.Country) &&
            EqualityComparer<string?>.Default.Equals(PostCode, other.PostCode)
            ;
    }
}

