using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Models;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralsByReferrerCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByReferrerCommand(string referrer, int? pageNumber, int? pageSize, string? searchText)
    {
        EmailAddress = referrer;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 1;
        SearchText = searchText;
    }

    public string EmailAddress { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
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
            .Include(x => x.Service)
            .ThenInclude(x => x.Organisation)

            .AsSplitQuery()
            .AsNoTracking()

            .Where(x => x.Referrer.EmailAddress == request.EmailAddress);

        if (entities == null)
        {
            throw new NotFoundException(nameof(Referral), request.EmailAddress);
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
            return new PaginatedList<ReferralDto>(pagelist, pagelist.Count, request.PageNumber, request.PageSize);
        }
       
        pagelist = _mapper.Map<List<ReferralDto>>(referralList);
        var result = new PaginatedList<ReferralDto>(pagelist.ToList(), pagelist.Count, 1, 10);
        return result;
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
