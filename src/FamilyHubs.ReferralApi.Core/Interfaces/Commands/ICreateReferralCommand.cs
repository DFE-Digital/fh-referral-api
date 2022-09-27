using FamilyHubs.ServiceDirectory.Shared.Models.Api.Referrals;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Commands;

public interface ICreateReferralCommand
{
    public ReferralDto ReferralDto { get; }
}
