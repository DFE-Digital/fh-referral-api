﻿using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Enums;
using FamilyHubs.ReferralService.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralsByReferrerByReferrerIdCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByReferrerByReferrerIdCommand(long id, ReferralOrderBy? orderBy, bool? isAssending, int? pageNumber, int? pageSize)
    {
        Id = id;
        OrderBy = orderBy;
        IsAssending = isAssending;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 10;
    }

    public long Id { get; set; }
    public ReferralOrderBy? OrderBy { get; init; }
    public bool? IsAssending { get; init; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetReferralsByReferrerByReferrerIdCommandHandler : GetReferralsHandlerBase, IRequestHandler<GetReferralsByReferrerByReferrerIdCommand, PaginatedList<ReferralDto>>
{
    public GetReferralsByReferrerByReferrerIdCommandHandler(ApplicationDbContext context, IMapper mapper)
        : base(context, mapper)
    {

    }
    public async Task<PaginatedList<ReferralDto>> Handle(GetReferralsByReferrerByReferrerIdCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.Referrals
            .Include(x => x.Status)
            .Include(x => x.Referrer)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.ReferralOrganisation)

            .AsSplitQuery()
            .AsNoTracking()

            .Where(x => x.Referrer.Id == request.Id);

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.Id.ToString());
        }

        entities = OrderBy(entities, request.OrderBy, request.IsAssending);

        return await GetPaginatedList(request == null, entities, request?.PageNumber ?? 1, request?.PageSize ?? 10);
    }
}
