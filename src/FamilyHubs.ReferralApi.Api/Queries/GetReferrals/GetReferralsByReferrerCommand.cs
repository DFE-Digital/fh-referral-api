using Ardalis.GuardClauses;
using Ardalis.Specification;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralsByReferrerCommand : IRequest<PaginatedList<ReferralDto>>
{
    public GetReferralsByReferrerCommand(string referrer, int? pageNumber, int? pageSize, string? searchText, bool? doNotListRejected)
    {
        Referrer = referrer;
        PageNumber = pageNumber != null ? pageNumber.Value : 1;
        PageSize = pageSize != null ? pageSize.Value : 1;
        SearchText = searchText;
        DoNotListRejected = doNotListRejected;
    }

    public string Referrer { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
    public bool? DoNotListRejected { get; set; }
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

        if (request != null && !string.IsNullOrEmpty(request.SearchText))
        {
            entities = entities.Where(x => x.RequestNumber.ToString().Contains(request.SearchText) ||
                                           (x.DateRecieved != null && x.DateRecieved.Value.ToString("dd MMMM yyyy").Contains(request.SearchText)) ||
                                           x.FullName.Contains(request.SearchText));
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
            string.Empty,
            x.DateRecieved,
            x.RequestNumber,
            x.ReasonForSupport,
            x.ReasonForRejection,
            x.Status.Select(x => new ReferralStatusDto(x.Id, x.Status)).ToList()
            )).ToListAsync(cancellationToken);

            if (request != null && request.DoNotListRejected != null && request.DoNotListRejected == true)
            {
                filteredReferrals = filteredReferrals.Where(x => !IsRejected(x.Status)).ToList();
            }

            if (request != null)
            {
                var pageList = filteredReferrals.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                var result = new PaginatedList<ReferralDto>(pageList, filteredReferrals.Count, request.PageNumber, request.PageSize);
                return result;
            }
        

        return new PaginatedList<ReferralDto>(filteredReferrals, filteredReferrals.Count, 1, 10);
    }

    private bool IsRejected(ICollection<ReferralStatusDto> status)
    {
        if (status == null || !status.Any())
            return false;

        ReferralStatusDto referralStatus = status.LastOrDefault() ?? default!;
        if (referralStatus != null && (referralStatus.Status.Contains("Reject") || referralStatus.Status.Contains("Decline")))
        {
            return true;
        }

        return false;

    }
}
