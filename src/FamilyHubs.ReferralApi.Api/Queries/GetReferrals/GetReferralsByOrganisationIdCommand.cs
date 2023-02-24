using Ardalis.GuardClauses;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralsByOrganisationIdCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByOrganisationIdCommand(string organisationId, int? pageNumber, int? pageSize)
    {
        OrganisationId = organisationId;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 1;
    }

    public string OrganisationId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetReferralsByOrganisationIdCommandHandler : IRequestHandler<GetReferralsByOrganisationIdCommand, PaginatedList<ReferralDto>>
{
    private readonly ApplicationDbContext _context;

    public GetReferralsByOrganisationIdCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedList<ReferralDto>> Handle(GetReferralsByOrganisationIdCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.Referrals
            .Include(x => x.Status)
            .Where(x => x.OrganisationId == request.OrganisationId);

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.OrganisationId);
        }

        var filteredReferrals = await entities.Select(x => new ReferralDto(
            x.Id,
            x.OrganisationId,
            x.ServiceId,
            x.ServiceName,
            x.ServiceDescription,
            x.ServiceAsJson,
            x.Referrer,
            x.FullName,
            x.HasSpecialNeeds,
            x.Email,
            x.Phone,
            x.Text,
            x.ReasonForSupport,
            x.ReasonForRejection,
            x.Status.Select(x => new ReferralStatusDto(x.Id, x.Status)).ToList()
            )).ToListAsync(cancellationToken);

        if (request != null)
        {
            var pagelist = filteredReferrals.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            var result = new PaginatedList<ReferralDto>(filteredReferrals, pagelist.Count, request.PageNumber, request.PageSize);
            return result;
        }

        return new PaginatedList<ReferralDto>(filteredReferrals, filteredReferrals.Count, 1, 10);


    }
}
