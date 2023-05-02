using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.SetReferralStatus;

public class SetReferralStatusCommand: IRequest<string>, ISetReferralStatusCommand
{
    public SetReferralStatusCommand(long referralId, string status)
    {
        Status = status;
        ReferralId = referralId;
    }

    public long ReferralId { get; }

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

            var entity = new ReferralStatus
            {
                ReferralId = request.ReferralId,
                Status = request.Status,
            };
            
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
