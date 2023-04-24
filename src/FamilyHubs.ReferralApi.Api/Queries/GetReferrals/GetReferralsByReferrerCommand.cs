using Ardalis.GuardClauses;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralsByReferrerCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByReferrerCommand(string referrer, int? pageNumber, int? pageSize)
    {
        Referrer = referrer;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 1;
    }

    public string Referrer { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetReferralsByReferrerCommandHandler : IRequestHandler<GetReferralsByReferrerCommand, PaginatedList<ReferralDto>>
{
    private readonly ApplicationDbContext _context;

    public GetReferralsByReferrerCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedList<ReferralDto>> Handle(GetReferralsByReferrerCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.Referrals
            .Include(x => x.Status)
            .Where(x => x.Referrer == request.Referrer);

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.Referrer);
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
            x.Created.Value,
            0L,
            x.ReasonForSupport,
            x.ReasonForRejection,
            x.Status.Select(x => new ReferralStatusDto(x.Id, x.Status)).ToList()
            )).ToListAsync(cancellationToken);

        if (request != null)
        {
            var pagelist = filteredReferrals.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            var result = new PaginatedList<ReferralDto>(filteredReferrals, pagelist.Count(), request.PageNumber, request.PageSize);
            return result;
        }

        return new PaginatedList<ReferralDto>(filteredReferrals, filteredReferrals.Count(), 1, 10);


    }
}
