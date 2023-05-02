using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface IUpdateReferralCommand
{
    public ReferralDto ReferralDto { get; }
}
