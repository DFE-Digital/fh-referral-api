using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.SharedKernel.Identity.Models;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface ICreateReferralCommand
{
    CreateReferralDto CreateReferral { get; }
    FamilyHubsUser FamilyHubsUser { get; }
}
