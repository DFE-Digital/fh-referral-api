using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface ICreateUserAccountsCommand
{
    List<UserAccountDto> UserAccounts { get; }
}
