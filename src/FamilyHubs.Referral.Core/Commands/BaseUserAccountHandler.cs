using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using Microsoft.IdentityModel.Tokens;

namespace FamilyHubs.Referral.Core.Commands;

public abstract class BaseUserAccountHandler
{
    protected readonly ApplicationDbContext _context;

    protected BaseUserAccountHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    protected async Task<UserAccount> AttatchExistingUserAccountRoles(UserAccount entity, CancellationToken cancellationToken)
    {
        if (entity.UserAccountRoles == null)
        {
            return entity;
        }
        for (int i = 0; i < entity.UserAccountRoles.Count; i++)
        {
            UserAccountRole? dbUserAccountRole = _context.UserAccountRoles.SingleOrDefault(x => x.UserAccountId == entity.UserAccountRoles[i].UserAccountId && (x.RoleId == entity.UserAccountRoles[i].RoleId || x.Role.Name == entity.UserAccountRoles[i].Role.Name));
            if (dbUserAccountRole == null)
            {
                _context.UserAccountRoles.Add(entity.UserAccountRoles[i]);
                await _context.SaveChangesAsync(cancellationToken);
            }

            entity.UserAccountRoles[i] = _context.UserAccountRoles.Single(x => x.UserAccountId == entity.UserAccountRoles[i].UserAccountId && x.RoleId == entity.UserAccountRoles[i].RoleId);
        }

        return entity;
    }

    protected async Task<UserAccount> AttatchExistingOrgansiation(UserAccount entity, CancellationToken cancellationToken)
    {
        if (entity.OrganisationUserAccounts == null)
        {
            return entity;
        }
        for(int i = 0; i < entity.OrganisationUserAccounts.Count; i++)
        {
            Organisation? organisation = _context.Organisations.SingleOrDefault(x => x.Id == entity.OrganisationUserAccounts[i].Organisation.Id);

            if (organisation == null)
            {
                if (string.IsNullOrEmpty(entity.OrganisationUserAccounts[i].Organisation.Name))
                {
                    entity.OrganisationUserAccounts.RemoveAt(i);
                    i--;
                    continue;
                }
                _context.Organisations.Add(entity.OrganisationUserAccounts[i].Organisation);
                await _context.SaveChangesAsync(cancellationToken);
            }

            entity.OrganisationUserAccounts[i].Organisation = _context.Organisations.Single(x => x.Id == entity.OrganisationUserAccounts[i].Organisation.Id);
        }

        return entity;
    }

    protected async Task<UserAccount> AttatchExistingService(UserAccount entity, CancellationToken cancellationToken)
    {
        if (entity.ServiceUserAccounts == null)
        {
            return entity;
        }
        foreach (UserAccountService serviceUserAccount in entity.ServiceUserAccounts)
        {
            Organisation? organisation = _context.Organisations.SingleOrDefault(x => x.Id == serviceUserAccount.ReferralService.Id);

            if (organisation == null)
            {
                _context.ReferralServices.Add(serviceUserAccount.ReferralService);
                await _context.SaveChangesAsync(cancellationToken);
            }

            serviceUserAccount.ReferralService = _context.ReferralServices.Single(x => x.Id == serviceUserAccount.ReferralService.Id);
        }

        return entity;
    }
}
