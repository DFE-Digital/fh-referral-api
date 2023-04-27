using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.ReferralApi.Data.Entities;
using FamilyHubs.ReferralApi.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Core.Queries.GetReferrals;

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

            .AsSplitQuery()
            .AsNoTracking()

            .Where(x => x.Referrer.EmailAddress == request.EmailAddress);

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.EmailAddress);
        }


        List<ReferralDto> pagelist;
        if (request != null)
        {
            pagelist = await  entities.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                .ProjectTo<ReferralDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new PaginatedList<ReferralDto>(pagelist, pagelist.Count, request.PageNumber, request.PageSize);
        }
       
        pagelist = _mapper.Map<List<ReferralDto>>(entities);
        var result = new PaginatedList<ReferralDto>(pagelist.ToList(), pagelist.Count, 1, 10);
        return result;
    }
}
