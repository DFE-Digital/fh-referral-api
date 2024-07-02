using FamilyHubs.ReferralService.Shared.CreateUpdateDto;
using FamilyHubs.SharedKernel.Identity.Models;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface ICreateReferralCommand
{
    CreateReferralDto CreateReferral { get; }
    FamilyHubsUser FamilyHubsUser { get; }
}
