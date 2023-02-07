using AutoMapper;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Events;
using FamilyHubs.ReferralApi.Core.Interfaces.Commands;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto;
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
            CreateReferral(request);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

        if (request is not null && request.ReferralDto is not null)
            return request.ReferralDto.Id;
        else
            return string.Empty;
    }

    private void CreateReferral(CreateReferralCommand request)
    {
        var entity = _mapper.Map<Referral>(request.ReferralDto);
        ArgumentNullException.ThrowIfNull(entity);

        if (entity.Status != null)
        {
            for (int i = entity.Status.Count - 1; i >= 0; i--)
            {
                var referralStatus = _context.ReferralStatuses.FirstOrDefault(x => x.Id == entity.Status.ElementAt(i).Id);
                if (referralStatus != null)
                {
                    entity.Status.Remove(entity.Status.ElementAt(i));
                    entity.Status.Add(referralStatus);
                }
            }
        }

        entity.RegisterDomainEvent(new ReferralCreatedEvent(entity));
        _context.Referrals.Add(entity);
    }
}

