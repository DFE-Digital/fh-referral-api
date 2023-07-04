using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface IUpdateUserAccountsCommand
{
    List<UserAccountDto> UserAccounts { get; }
}
