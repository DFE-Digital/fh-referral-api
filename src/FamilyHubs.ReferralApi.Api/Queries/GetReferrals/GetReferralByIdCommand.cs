using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

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
        var entity = await _context.Referrals
            .Include(x => x.Status)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.Id.ToString());
        }

        List<ReferralStatusDto> referralStatusDtos = new();
        foreach (var status in entity.Status)
        {
            referralStatusDtos.Add(new ReferralStatusDto
            {
                Status = status.Status,
                Id = status.Id
            });
        }

        ReferralDto result = _mapper.Map<ReferralDto>(entity);

        return result;
    }
}

