using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Events;
using FamilyHubs.ReferralApi.Core.Interfaces.Commands;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using MediatR;

namespace FamilyHubs.ReferralApi.Api.Commands.SetReferralStatus;

public class SetReferralStatusCommand: IRequest<string>, ISetReferralStatusCommand
{
    public SetReferralStatusCommand(string referralId, string status)
    {
        Status = status;
        ReferralId = referralId;
    }

    public string ReferralId { get; }

    public string Status { get; }
}

public class CreateReferralStatusCommandHandler : IRequestHandler<SetReferralStatusCommand, string>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateReferralStatusCommandHandler> _logger;
    public CreateReferralStatusCommandHandler(ApplicationDbContext context, ILogger<CreateReferralStatusCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<string> Handle(SetReferralStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentStatus = _context.ReferralStatuses.Where(x => x.ReferralId == request.ReferralId).OrderBy(x => x.LastModified).LastOrDefault();
            if (currentStatus != null && currentStatus.Status == request.Status) 
            { 
                return currentStatus.Status;
            }
            var entity = new ReferralStatus(Guid.NewGuid().ToString(), request.Status, request.ReferralId);
            entity.RegisterDomainEvent(new ReferralStatusCreatedEvent(entity));
            _context.ReferralStatuses.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

        if (request is not null && request.Status is not null)
            return request.Status;
        else
            return string.Empty;
    }
}
