using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;

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
            UserAccountRole? dbUserAccountRole = _context.UserAccountRoles.SingleOrDefault(x => x.UserAccountId == entity.UserAccountRoles[i].UserAccountId && x.RoleId == entity.UserAccountRoles[i].RoleId);
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
        foreach (UserAccountOrganisation organisationUserAccount in entity.OrganisationUserAccounts)
        {
            Organisation? organisation = _context.Organisations.SingleOrDefault(x => x.Id == organisationUserAccount.Organisation.Id);

            if (organisation == null)
            {
                _context.Organisations.Add(organisationUserAccount.Organisation);
                await _context.SaveChangesAsync(cancellationToken);
            }

            organisationUserAccount.Organisation = _context.Organisations.Single(x => x.Id == organisationUserAccount.Organisation.Id);
        }

        return entity;
    }
}
