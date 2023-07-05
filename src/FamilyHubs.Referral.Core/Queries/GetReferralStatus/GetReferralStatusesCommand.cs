using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferralStatus;

public class GetReferralStatusesCommand : IRequest<List<ReferralStatusDto>>
{
    public GetReferralStatusesCommand()
    {
        
    }
}

public class GetReferralStatusesCommandHandler : IRequestHandler<GetReferralStatusesCommand, List<ReferralStatusDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReferralStatusesCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<ReferralStatusDto>> Handle(GetReferralStatusesCommand request, CancellationToken cancellationToken)
    {
        var entities = _context.Statuses
            .ProjectTo<ReferralStatusDto>(_mapper.ConfigurationProvider)
            .AsNoTracking();

        if (entities == null)
        {
            throw new NotFoundException(nameof(Status), "ReferralStatusList");
        }


        return await entities.ToListAsync();
    }
}
