using AutoMapper;
using AutoMapper.QueryableExtensions;
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

    protected IQueryable<Data.Entities.Referral> OrderBy(IQueryable<Data.Entities.Referral> currentList, ReferralOrderBy? orderBy, bool? isAssending) 
    {
        if (orderBy == null || isAssending == null)
            return currentList;

        switch(orderBy) 
        {
            case ReferralOrderBy.Team:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.Referrer.Team);
                return currentList.OrderByDescending(x => x.Referrer.Team);

            case ReferralOrderBy.DateSent:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.Created);
                return currentList.OrderByDescending(x => x.Created);

            case ReferralOrderBy.DateUpdated:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.LastModified);
                return currentList.OrderByDescending(x => x.LastModified);

            case ReferralOrderBy.Status:
                if (isAssending.Value)
                    return currentList.OrderBy(x => x.Status.SortOrder);
                return currentList.OrderByDescending(x => x.Status.SortOrder);

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
