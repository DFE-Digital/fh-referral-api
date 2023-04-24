using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;
using FamilyHubs.ServiceDirectory.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralsByOrganisationIdCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByOrganisationIdCommand(long organisationId, int? pageNumber, int? pageSize)
    {
        OrganisationId = organisationId;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 1;
    }

    public long OrganisationId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetReferralsByOrganisationIdCommandHandler : IRequestHandler<GetReferralsByOrganisationIdCommand, PaginatedList<ReferralDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReferralsByOrganisationIdCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<ReferralDto>> Handle(GetReferralsByOrganisationIdCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.Referrals
            .Include(x => x.Status)
            .Include(x => x.Referrer)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.ReferralOrganisation)
            .ProjectTo<ReferralDto>(_mapper.ConfigurationProvider)
            .Where(x => x.ReferralServiceDto.ReferralOrganisationDto.Id == request.OrganisationId);

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.OrganisationId.ToString());
        }

        

        if (request != null)
        {
            var pagelist = entities.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            var result = new PaginatedList<ReferralDto>(entities.ToList(), pagelist.Count, request.PageNumber, request.PageSize);
            return result;
        }

        return new PaginatedList<ReferralDto>(entities.ToList(), entities.Count(), 1, 10);


    }
}
