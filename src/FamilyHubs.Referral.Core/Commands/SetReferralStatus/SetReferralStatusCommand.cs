using Ardalis.GuardClauses;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.SetReferralStatus;

public class SetReferralStatusCommand: IRequest<string>, ISetReferralStatusCommand
{
    public SetReferralStatusCommand(long referralId, string status, string reasonForDecliningSupport)
    {
        Status = status;
        ReferralId = referralId;
        ReasonForDecliningSupport = reasonForDecliningSupport;
    }

    public long ReferralId { get; }

    public string Status { get; }

    public string ReasonForDecliningSupport { get; }

}

public class SetReferralStatusCommandHandler : IRequestHandler<SetReferralStatusCommand, string>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SetReferralStatusCommandHandler> _logger;
    public SetReferralStatusCommandHandler(ApplicationDbContext context, ILogger<SetReferralStatusCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<string> Handle(SetReferralStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _context.Referrals
            .Include(x => x.Status)
            .Include(x => x.Referrer)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.ReferralOrganisation)
            .FirstOrDefaultAsync(p => p.Id == request.ReferralId, cancellationToken: cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Referral), request.ReferralId.ToString());
            }

            var updatedStatus = _context.ReferralStatuses.SingleOrDefault(x => x.Name == request.Status);

            if (updatedStatus == null)
            {
                throw new NotFoundException(nameof(ReferralStatus), request.Status);
            }

            entity.ReasonForDecliningSupport = request.ReasonForDecliningSupport;
            entity.StatusId = updatedStatus.Id;
            entity.Status = updatedStatus;
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Status.Name;
           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

    }
}
