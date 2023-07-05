using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface ICreateUserAccountCommand
{
    UserAccountDto UserAccount { get; }
}
