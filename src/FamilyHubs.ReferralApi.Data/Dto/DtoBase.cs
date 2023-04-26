namespace FamilyHubs.ReferralCommon.Shared.Dto;

//Will be moved into a seperate NuGet Package

public record DtoBase<TId>
{
#pragma warning disable CS8618
    public TId Id { get; set; }
#pragma warning restore CS8618
}