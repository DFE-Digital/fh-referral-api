using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Enums;
using FamilyHubs.ReferralService.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand(long? serviceId, long? statusId, long? recipientId, long? referralId, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize)
    {
        ServiceId = serviceId; 
        StatusId = statusId;
        RecipientId = recipientId;
        ReferralId = referralId;
        OrderBy = orderBy;
        IsAssending = isAssending;
        IncludeDeclined = includeDeclined;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 10;
    }

    public long? ServiceId { get; init; }
    public long? StatusId { get; init; }
    public long? ReferralId { get; init; }
    public long? RecipientId { get; init; }
    public ReferralOrderBy? OrderBy { get; init; }
    public bool? IsAssending { get; init; }
    public bool? IncludeDeclined { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommandHandler : GetReferralsHandlerBase, IRequestHandler<GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand, PaginatedList<ReferralDto>>
{
    public GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommandHandler(ApplicationDbContext context, IMapper mapper)
    : base(context, mapper)
    {

    }
    public async Task<PaginatedList<ReferralDto>> Handle(GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.Referrals
            .Include(x => x.Status)
            .Include(x => x.UserAccount)
            .ThenInclude(x => x.OrganisationUserAccounts)
            .Include(x => x.UserAccount)
            .ThenInclude(x => x.ServiceUserAccounts)

            .Include(x => x.UserAccount)
            .ThenInclude(x => x.UserAccountRoles)

            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.Organisation)
            .AsNoTracking();

        if (request.ServiceId != null)
        {
            entities = entities.Where(x => x.ReferralService.Id == request.ServiceId);
        }

        if (request.StatusId != null)
        {
            entities = entities.Where(x => x.Status.Id == request.StatusId);
        }

        if (request.RecipientId != null)
        {
            entities = entities.Where(x => x.Recipient.Id == request.RecipientId);
        }

        if (request.ReferralId != null)
        {
            entities = entities.Where(x => x.Id == request.ReferralId);
        }

        if (request.IncludeDeclined == null || request.IncludeDeclined == false)
        {
            entities = entities.Where(x => x.Status.Name != "Declined");
        }

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), "Id");
        }

        entities = OrderBy(entities, request.OrderBy, request.IsAssending);

        return await GetPaginatedList(request == null, entities, request?.PageNumber ?? 1, request?.PageSize ?? 10);

    }
}


