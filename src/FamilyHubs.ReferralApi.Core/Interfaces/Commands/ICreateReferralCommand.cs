using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Commands;

public interface ICreateReferralCommand
{
    public ReferralDto ReferralDto { get; }
}
