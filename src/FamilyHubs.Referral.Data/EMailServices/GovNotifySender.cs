using FamilyHubs.ReferralService.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notify.Client;
using Notify.Interfaces;

namespace FamilyHubs.Referral.Data.EMailServices;

public class GovNotifySender : IEmailSender
{
    private readonly IOptions<GovNotifySetting> _govNotifySettings;
    private readonly IAsyncNotificationClient _notificationClient;

    public GovNotifySender(IAsyncNotificationClient notificationClient, IOptions<GovNotifySetting> govNotifySettings)
    {
        _notificationClient = notificationClient;
        _govNotifySettings = govNotifySettings;
    }

    public async Task SendEmailAsync(MessageDto messageDto)
    {
        Dictionary<String, dynamic> personalisation = new Dictionary<string, dynamic>();
        foreach(KeyValuePair<string,string> token in messageDto.TemplateTokens)
        {
            personalisation.Add(token.Key, token.Value);
        }

        await _notificationClient.SendEmailAsync(
                emailAddress: messageDto.RecipientEmail,
                templateId: !string.IsNullOrEmpty(messageDto.TemplateId) ? messageDto.TemplateId : _govNotifySettings.Value.TemplateId,
                personalisation: personalisation,
                clientReference: null,
                emailReplyToId: null
        );
    }
}


