using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Commands;

public interface IUpdateReferralCommand
{
    public ReferralDto ReferralDto { get; }
}
