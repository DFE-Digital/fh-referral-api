using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Enums;
using FamilyHubs.ReferralService.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public abstract class GetReferralsHandlerBase
{
    protected readonly ApplicationDbContext _context;
    protected readonly IMapper _mapper;
    protected GetReferralsHandlerBase(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    protected async Task<PaginatedList<ReferralDto>> GetPaginatedList(bool requestIsNull, IQueryable<Data.Entities.Referral> referralList, int pageNumber, int pageSize)
    {
        int totalRecords = referralList.Count();
        List<ReferralDto> pagelist;
        if (!requestIsNull)
        {
            pagelist = await referralList.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .ProjectTo<ReferralDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new PaginatedList<ReferralDto>(pagelist, totalRecords, pageNumber, pageSize);
        }

        pagelist = _mapper.Map<List<ReferralDto>>(referralList);
        var result = new PaginatedList<ReferralDto>(pagelist.ToList(), totalRecords, 1, 10);
        return result;
    }

    protected async Task<PaginatedList<UserAccountDto>> GetPaginatedList(bool requestIsNull, IQueryable<UserAccount> userAccounts, int pageNumber, int pageSize)
    {
        int totalRecords = userAccounts.Count();
        List<UserAccountDto> pagelist;
        if (!requestIsNull)
        {
            pagelist = await userAccounts.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .ProjectTo<UserAccountDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            foreach(UserAccountDto userAccount in pagelist)
            {
                UserAccount? dbUserAccount = userAccounts.FirstOrDefault(x => x.Id == userAccount.Id);
                if (dbUserAccount != null && dbUserAccount.OrganisationUserAccounts != null)
                {
                    userAccount.OrganisationUserAccounts = new List<UserAccountOrganisationDto>();
                    foreach (var organisationUserAccount in dbUserAccount.OrganisationUserAccounts)
                    {
                        var organisation = _mapper.Map<OrganisationDto>(organisationUserAccount.Organisation);
                        organisationUserAccount.UserAccount.OrganisationUserAccounts = null;
                        var user = _mapper.Map<UserAccountDto>(organisationUserAccount.UserAccount);
                 
                        userAccount.OrganisationUserAccounts.Add(new UserAccountOrganisationDto()
                        {
                            UserAccount = user,
                            Organisation = organisation
                        });
                    }
                    
                }
            }

            return new PaginatedList<UserAccountDto>(pagelist, totalRecords, pageNumber, pageSize);
        }

        pagelist = _mapper.Map<List<UserAccountDto>>(userAccounts);
        var result = new PaginatedList<UserAccountDto>(pagelist.ToList(), totalRecords, 1, 10);
        return result;
    }

    protected IQueryable<Data.Entities.Referral> OrderBy(IQueryable<Data.Entities.Referral> currentList, ReferralOrderBy? orderBy, bool? isAssending, bool isByReferrer = false) 
    {
        if (orderBy == null || isAssending == null)
            return currentList;

        switch(orderBy) 
        {
            case ReferralOrderBy.Team:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.UserAccount.Team);
                return currentList.OrderByDescending(x => x.UserAccount.Team);

            case ReferralOrderBy.DateSent:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.Created);
                return currentList.OrderByDescending(x => x.Created);

            case ReferralOrderBy.DateUpdated:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.LastModified);
                return currentList.OrderByDescending(x => x.LastModified);

            case ReferralOrderBy.Status:
                if (isByReferrer)
                {
                    if (isAssending.Value)
                        return currentList.OrderBy(x => x.Status.SecondrySortOrder).ThenByDescending(x => x.LastModified);
                    return currentList.OrderByDescending(x => x.Status.SecondrySortOrder).ThenByDescending(x => x.LastModified);
                }
                else
                {
                    if (isAssending.Value)
                        return currentList.OrderBy(x => x.Status.SortOrder).ThenByDescending(x => x.Created);
                    return currentList.OrderByDescending(x => x.Status.SortOrder).ThenByDescending(x => x.Created);
                }
                

            case ReferralOrderBy.RecipientName:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.Recipient.Name);
                return currentList.OrderByDescending(x => x.Recipient.Name);

            case ReferralOrderBy.ServiceName:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.ReferralService.Name);
                return currentList.OrderByDescending(x => x.ReferralService.Name);

        }

        return currentList;
    }
}
