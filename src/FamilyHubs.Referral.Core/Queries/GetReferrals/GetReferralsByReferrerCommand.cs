using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Enums;
using FamilyHubs.ReferralService.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralsByReferrerCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByReferrerCommand(string referrer, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize)
    {
        EmailAddress = referrer;
        OrderBy = orderBy;
        IsAssending = isAssending;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 10;
    }

    public string EmailAddress { get; set; }
    public ReferralOrderBy? OrderBy { get; init; }
    public bool? IsAssending { get; init; }
    public bool? IncludeDeclined { get; init; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetReferralsByReferrerCommandHandler : GetReferralsHandlerBase, IRequestHandler<GetReferralsByReferrerCommand, PaginatedList<ReferralDto>>
{
    public GetReferralsByReferrerCommandHandler(ApplicationDbContext context, IMapper mapper)
        : base(context, mapper)
    {
        
    }
    public async Task<PaginatedList<ReferralDto>> Handle(GetReferralsByReferrerCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.Referrals.GetAll()
            .AsSplitQuery()
            .AsNoTracking();

        if (request.IncludeDeclined != null && request.IncludeDeclined == true)
        {
            entities = entities.Where(x => x.UserAccount.EmailAddress == request.EmailAddress);
        }
        else
        {
            entities = entities.Where(x => x.UserAccount.EmailAddress == request.EmailAddress && x.Status.Name != "Declined");
        }
#pragma warning disable S2583
        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.EmailAddress);
        }
#pragma warning restore S2583
        entities = OrderBy(entities, request.OrderBy, request.IsAssending, true);

        return await GetPaginatedList(request == null, entities, request?.PageNumber ?? 1, request?.PageSize ?? 10);
    }
}
