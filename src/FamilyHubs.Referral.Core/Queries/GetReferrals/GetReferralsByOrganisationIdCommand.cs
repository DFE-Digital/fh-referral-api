using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Models;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralsByOrganisationIdCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByOrganisationIdCommand(long organisationId, int? pageNumber, int? pageSize, string? searchText)
    {
        OrganisationId = organisationId;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 1;
        SearchText = searchText;
    }

    public long OrganisationId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
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
            .AsNoTracking()
            .Where(x => x.ReferralService.ReferralOrganisation.Id == request.OrganisationId);

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.OrganisationId.ToString());
        }

        IEnumerable<Data.Entities.Referral> referralList = await entities.ToListAsync();
        if (request.SearchText != null)
        {
            referralList = FilterReferrals(referralList, request.SearchText);
        }

        List<ReferralDto> pagelist;
        if (request != null)
        {
            var list = referralList.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            pagelist = _mapper.Map<List<ReferralDto>>(list);
            var result = new PaginatedList<ReferralDto>(pagelist.ToList(), pagelist.Count, request.PageNumber, request.PageSize);
            return result;
        }
        
        pagelist = _mapper.Map<List<ReferralDto>>(entities);
        return new PaginatedList<ReferralDto>(pagelist.ToList(), pagelist.Count, 1, 10);
        
    }

    private List<Data.Entities.Referral> FilterReferrals(IEnumerable<Data.Entities.Referral> entities, string searchString)
    {
        if (searchString == null)
            return entities.ToList();

        var predicate = PredicateBuilder.New<Data.Entities.Referral>();

        string[] parts = searchString.Split(' ');

        foreach (var str in parts)
        {
            var tmp = str.ToLower();
            predicate = predicate.Or(o => o.Recipient.Name.ToLower().IndexOf(tmp) != -1).Or(x => x.Id.ToString("X").IndexOf(tmp) != 1);
        }

        var results = entities.Where(predicate).ToList();

        return results;
    }
}
