using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralByIdCommand : IRequest<ReferralDto>
{
    public GetReferralByIdCommand(long id)
    {
        Id = id;
    }

    public long Id { get; set; }

}

public class GetReferralByIdCommandHandler : IRequestHandler<GetReferralByIdCommand, ReferralDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReferralByIdCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ReferralDto> Handle(GetReferralByIdCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Referrals.GetAll()
            .AsNoTracking()
            .ProjectTo<ReferralDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.Id.ToString());
        }

        
        return entity;
    }
}

