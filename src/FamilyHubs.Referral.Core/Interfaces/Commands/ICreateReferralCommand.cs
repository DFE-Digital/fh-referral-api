using FamilyHubs.ReferralService.Shared.CreateUpdateDto;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface ICreateReferralCommand
{
    public CreateReferralDto CreateReferral { get; }
}
