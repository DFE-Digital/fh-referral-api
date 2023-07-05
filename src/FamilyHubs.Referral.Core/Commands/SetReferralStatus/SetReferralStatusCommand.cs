using Ardalis.GuardClauses;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.SharedKernel.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.SetReferralStatus;

public class SetReferralStatusCommand: IRequest<string>, ISetReferralStatusCommand
{
    public SetReferralStatusCommand(string role, long userOrganisationId, long referralId, string status, string reasonForDecliningSupport)
    {
        Status = status;
        ReferralId = referralId;
        ReasonForDecliningSupport = reasonForDecliningSupport;
        Role = role;
        UserOrganisationId = userOrganisationId;
    }
    public string Role { get; }

    public long UserOrganisationId { get; }
    public long ReferralId { get; }

    public string Status { get; }

    public string ReasonForDecliningSupport { get; }

}

public class SetReferralStatusCommandHandler : IRequestHandler<SetReferralStatusCommand, string>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SetReferralStatusCommandHandler> _logger;
    public static string Forbidden { get; } = "Forbidden";
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
            .Include(x => x.UserAccount)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.Organisation)
            .FirstOrDefaultAsync(p => p.Id == request.ReferralId, cancellationToken: cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Referral), request.ReferralId.ToString());
            }

            //Only modify Status if DfEAdmin or belong to VCS Organisation,
            //assumption is VCS Professional will have correct organisation id other users will not
            if (entity.ReferralService.Organisation.Id == request.UserOrganisationId || RoleTypes.DfeAdmin == request.Role) 
            {
                var updatedStatus = _context.Statuses.SingleOrDefault(x => x.Name == request.Status) ?? throw new NotFoundException(nameof(Status), request.Status);

                entity.ReasonForDecliningSupport = request.ReasonForDecliningSupport;
                entity.StatusId = updatedStatus.Id;
                entity.Status = updatedStatus;
                await _context.SaveChangesAsync(cancellationToken);

                return entity.Status.Name;
            }

            return Forbidden;
           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

    }
}
