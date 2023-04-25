using AutoMapper;
using FamilyHubs.ReferralApi.Data.Entities;
using FamilyHubs.ReferralApi.Core.Interfaces.Commands;
using FamilyHubs.ReferralApi.Data.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.ReferralApi.Core.Commands.CreateReferral;

public class CreateReferralCommand : IRequest<long>, ICreateReferralCommand
{
    public CreateReferralCommand(ReferralDto referralDto)
    {
        ReferralDto = referralDto;
    }

    public ReferralDto ReferralDto { get; }
}

public class CreateReferralCommandHandler : IRequestHandler<CreateReferralCommand, long>
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
    public async Task<long> Handle(CreateReferralCommand request, CancellationToken cancellationToken)
    {
        long id = 0;
        try
        {
            id = CreateReferral(request);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

        return id;
    }

    private long CreateReferral(CreateReferralCommand request)
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

        _context.Referrals.Add(entity);

        return entity.Id;
    }
}

