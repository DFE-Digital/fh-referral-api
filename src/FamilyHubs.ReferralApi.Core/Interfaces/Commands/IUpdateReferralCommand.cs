using FamilyHubs.ServiceDirectory.Shared.Models.Api.Referrals;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Commands;

public interface IUpdateReferralCommand
{
    public ReferralDto ReferralDto { get; }
}
