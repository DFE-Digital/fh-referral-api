using AutoMapper;
using FamilyHubs.ReferralApi.Common.Dto;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Events;
using FamilyHubs.ReferralApi.Core.Interfaces.Commands;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using MediatR;

namespace FamilyHubs.ReferralApi.Api.Commands.CreateReferral;

public class CreateReferralCommand : IRequest<string>, ICreateReferralCommand
{
    public CreateReferralCommand(ReferralDto referralDto)
    {
        ReferralDto = referralDto;
    }

    public ReferralDto ReferralDto { get; }
}

public class CreateReferralCommandHandler : IRequestHandler<CreateReferralCommand, string>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateReferralCommandHandler> _logger;
    public CreateReferralCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<CreateReferralCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }
    public async Task<string> Handle(CreateReferralCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<Referral>(request.ReferralDto);
#pragma warning disable S3236 // Caller information arguments should not be provided explicitly
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
#pragma warning restore S3236 // Caller information arguments should not be provided explicitly

            entity.RegisterDomainEvent(new ReferralCreatedEvent(entity));
            _context.Referrals.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
#pragma warning disable S112 // General exceptions should never be thrown
            throw new Exception(ex.Message, ex);
#pragma warning restore S112 // General exceptions should never be thrown
        }

        if (request is not null && request.ReferralDto is not null)
            return request.ReferralDto.Id;
        else
            return string.Empty;
    }
}

