using Ardalis.GuardClauses;
using FamilyHubs.Notification.Api.Contracts;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Data.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Commands.Notifications;

public class SendNotificationCommand : IRequest<bool> //, ISendNotificationCommand
{
    public SendNotificationCommand(long referralId, long organisationId)
    {
        ReferralId = referralId;
        OrganisationId = organisationId;
    }

    public long ReferralId { get; }
    public long OrganisationId { get; }
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationClientService _notificationClientService;
    private const string _templateKey = "";

    protected SendNotificationCommandHandler(ApplicationDbContext context, INotificationClientService notificationClientService)
    {
        _context = context;
        _notificationClientService = notificationClientService;
    }
    public async Task<bool> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        MessageDto messageDto = await GetMessages(request, cancellationToken);

        return await _notificationClientService.SendNotification(messageDto, new System.IdentityModel.Tokens.Jwt.JwtSecurityToken());
    }

    private async Task<MessageDto> GetMessages(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Referrals
            .Include(x => x.UserAccount)
            .ThenInclude(x => x.OrganisationUserAccounts)
            .Include(x => x.UserAccount)
            .ThenInclude(x => x.ServiceUserAccounts)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.Organisation)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ReferralId, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.ReferralId.ToString());
        }

        Dictionary<string, string> dicTokens = new Dictionary<string, string>()
        {
            { "reference number", entity.Id.ToString("X6")},
            { "RequestNumber", entity.Id.ToString("X6") },
            { "ServiceName", entity.ReferralService.Name },
            { "ViewConnectionRequestUrl",  entity.ReferralService.Url ?? string.Empty },
            { "Name of service", entity.ReferralService.Name },
            { "link to specific connection request", entity.ReferralService.Url ?? string.Empty }
        };


        MessageDto messageDto = new MessageDto
        {
            ApiKeyType = ApiKeyType.ConnectKey,
            NotificationEmails = new List<string> { entity.UserAccount.EmailAddress },
            TemplateId = _templateKey,
            TemplateTokens = dicTokens
        };
        


        var userentities = _context.UserAccountOrganisations
            .Include(x => x.UserAccount)
            .Include(x => x.Organisation)
            .AsNoTracking()
            .Where(x => x.OrganisationId == request.OrganisationId);

        List<string> emailAddress = userentities.Select(x => x.UserAccount.EmailAddress).ToList();

        messageDto.NotificationEmails.AddRange(emailAddress);

        return messageDto;
    }
}
