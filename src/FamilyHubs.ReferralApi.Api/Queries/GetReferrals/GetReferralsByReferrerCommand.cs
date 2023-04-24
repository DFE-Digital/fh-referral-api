using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;
using FamilyHubs.ServiceDirectory.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralsByReferrerCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByReferrerCommand(string referrer, int? pageNumber, int? pageSize)
    {
        EmailAddress = referrer;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 1;
    }

    public string EmailAddress { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetReferralsByReferrerCommandHandler : IRequestHandler<GetReferralsByReferrerCommand, PaginatedList<ReferralDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReferralsByReferrerCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<ReferralDto>> Handle(GetReferralsByReferrerCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.Referrals
            .Include(x => x.Status)
            .Include(x => x.Referrer)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.ReferralOrganisation)
            .ProjectTo<ReferralDto>(_mapper.ConfigurationProvider)
            .Where(x => x.ReferrerDto.EmailAddress == request.EmailAddress);

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.EmailAddress);
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
