using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Enums;
using FamilyHubs.ReferralService.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetUserAccounts;


public class GetUsersByOrganisationIdCommand : IRequest<PaginatedList<UserAccountDto>>
{
    public GetUsersByOrganisationIdCommand(long organisationId, int? pageNumber, int? pageSize)
    {
        OrganisationId = organisationId;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 10;
    }

    public long OrganisationId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetUsersByOrganisationIdCommandHandler : GetReferralsHandlerBase, IRequestHandler<GetUsersByOrganisationIdCommand, PaginatedList<UserAccountDto>>
{
    public GetUsersByOrganisationIdCommandHandler(ApplicationDbContext context, IMapper mapper)
         : base(context, mapper)
    {

    }

    public async Task<PaginatedList<UserAccountDto>> Handle(GetUsersByOrganisationIdCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.UserAccounts
            .Include(x => x.OrganisationUserAccounts!)
            .ThenInclude(x => x.Organisation)
            .Where(x => x.OrganisationUserAccounts != null && x.OrganisationUserAccounts.Any(x => x.OrganisationId == request.OrganisationId))
            .AsNoTracking();

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.OrganisationId.ToString());
        }

        return await GetPaginatedList(request == null, entities, request?.PageNumber ?? 1, request?.PageSize ?? 10);

    }
}