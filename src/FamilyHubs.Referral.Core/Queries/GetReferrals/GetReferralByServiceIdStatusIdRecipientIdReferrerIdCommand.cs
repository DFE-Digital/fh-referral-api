using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand : IRequest<List<ReferralDto>>
{
    public GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand(long? serviceId, long? statusId, long? recipientId, long? referralId)
    {
        ServiceId = serviceId; 
        StatusId = statusId;
        RecipientId = recipientId;
        ReferralId = referralId;
    }

    public long? ServiceId { get; init; }
    public long? StatusId { get; init; }
    public long? ReferralId { get; init; }
    public long? RecipientId { get; init; }
}

public class GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommandHandler : IRequestHandler<GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand, List<ReferralDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<ReferralDto>> Handle(GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.Referrals
            .Include(x => x.Status)
            .Include(x => x.Referrer)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.ReferralOrganisation)
            .AsNoTracking();

        if (request.ServiceId != null)
        {
            entity = entity.Where(x => x.ReferralService.Id == request.ServiceId);
        }

        if (request.StatusId != null)
        {
            entity = entity.Where(x => x.Status.Id == request.StatusId);
        }

        if (request.RecipientId != null)
        {
            entity = entity.Where(x => x.Recipient.Id == request.RecipientId);
        }

        if (request.ReferralId != null)
        {
            entity = entity.Where(x => x.Id == request.ReferralId);
        }

        var result = await entity.ProjectTo<ReferralDto>(_mapper.ConfigurationProvider)
                           .ToListAsync(cancellationToken: cancellationToken);
        

        if (result == null)
        {
            throw new NotFoundException(nameof(Referral), "Id");
        }


        return result;
    }
}


