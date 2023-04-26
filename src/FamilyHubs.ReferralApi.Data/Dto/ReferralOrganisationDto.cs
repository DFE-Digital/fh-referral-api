// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable CS8604

//Will be moved into a seperate NuGet Package

namespace FamilyHubs.ReferralCommon.Shared.Dto;

public record ReferralOrganisationDto : DtoBase<long>
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public override int GetHashCode()
    {
        return
            EqualityComparer<string>.Default.GetHashCode(Name) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(Description) * -1521134295
            ;
    }


    public virtual bool Equals(ReferralOrganisationDto? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other))
            return true;

        return
            EqualityComparer<string>.Default.Equals(Name, other.Name) &&
            EqualityComparer<string?>.Default.Equals(Description, other.Description)
            ;
    }
}

