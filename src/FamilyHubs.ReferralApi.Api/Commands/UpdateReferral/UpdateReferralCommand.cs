using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Interfaces.Commands;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;
using MediatR;

namespace FamilyHubs.ReferralApi.Api.Commands.UpdateReferral;

public class UpdateReferralCommand : IRequest<long>, IUpdateReferralCommand
{
    public UpdateReferralCommand(long id, ReferralDto referralDto)
    {
        Id = id;
        ReferralDto = referralDto;
    }

    public long Id { get; }
    public ReferralDto ReferralDto { get; }
}

public class UpdateReferralCommandHandler : IRequestHandler<UpdateReferralCommand, long>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateReferralCommandHandler> _logger;
    public UpdateReferralCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<UpdateReferralCommandHandler> logger)
    {
        _logger = logger;
        _mapper = mapper;
        _context = context;
    }
    public async Task<long> Handle(UpdateReferralCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.Referrals.FirstOrDefault(x => x.Id == request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.Id.ToString());
        }

        try
        {
            entity = _mapper.Map(request.ReferralDto, entity);

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

        return entity.Id;
    }
}

