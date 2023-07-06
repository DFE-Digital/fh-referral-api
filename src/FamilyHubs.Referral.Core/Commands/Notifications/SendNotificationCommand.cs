using Ardalis.GuardClauses;
using FamilyHubs.Notification.Api.Contracts;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FamilyHubs.Referral.Core.Commands.Notifications;

public class SendNotificationCommand : IRequest<bool>, ISendNotificationCommand
{
    public SendNotificationCommand(long referralId)
    {
        ReferralId = referralId;
    }

    public long ReferralId { get; }
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationClientService _notificationClientService;
    private readonly IConfiguration _configuration;

    protected SendNotificationCommandHandler(ApplicationDbContext context, INotificationClientService notificationClientService, IConfiguration configuration)
    {
        _context = context;
        _notificationClientService = notificationClientService;
        _configuration = configuration;
    }
    public async Task<bool> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        MessageDto messageDto = await GetMessages(request, cancellationToken);

        return await _notificationClientService.SendNotification(messageDto, GetToken());
    }

    private async Task<MessageDto> GetMessages(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        string templateId = _configuration["ProfessionalSentRequest"] ?? string.Empty;

        if (string.IsNullOrEmpty(templateId)) 
        {
            throw new NotFoundException(nameof(Referral), $"Referral: {request.ReferralId.ToString()} - {templateId}" );
        }

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
            TemplateId = templateId,
            TemplateTokens = dicTokens
        };
        
        var userentities = _context.UserAccountOrganisations
            .Include(x => x.UserAccount)
            .Include(x => x.Organisation)
            .AsNoTracking()
            .Where(x => x.OrganisationId == entity.ReferralService.Organisation.Id);

        List<string> emailAddress = userentities.Select(x => x.UserAccount.EmailAddress).ToList();

        messageDto.NotificationEmails.AddRange(emailAddress);

        return messageDto;
    }

    private JwtSecurityToken GetToken()
    {
        var jti = Guid.NewGuid().ToString();
        var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_configuration["GovUkOidcConfiguration:BearerTokenSigningKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(
            claims: new List<Claim>
               {
                    new Claim("sub", _configuration["GovUkOidcConfiguration:Oidc:ClientId"] ?? ""),
                    new Claim("jti", jti),
                    new Claim(ClaimTypes.Role, "VcsProfessional")

               },
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(10)
            );

        return token;
    }
}
