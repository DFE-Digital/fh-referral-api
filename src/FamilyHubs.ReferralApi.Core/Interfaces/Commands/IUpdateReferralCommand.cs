using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Commands;

public interface IUpdateReferralCommand
{
    public ReferralDto ReferralDto { get; }
}
