using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries;

public static class QueryableExtensions
{
    public static IQueryable<Data.Entities.Referral> GetAll(this DbSet<Data.Entities.Referral> referralcontext)
    {
        return referralcontext
            .Include(x => x.Status)
            .Include(x => x.UserAccount)
            .ThenInclude(x => x.OrganisationUserAccounts)
            .Include(x => x.UserAccount)
            .ThenInclude(x => x.ServiceUserAccounts)

            .Include(x => x.UserAccount)
            .ThenInclude(x => x.UserAccountRoles)

            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.Organisation);
    }
}
