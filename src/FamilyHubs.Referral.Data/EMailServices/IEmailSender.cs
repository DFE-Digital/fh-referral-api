namespace FamilyHubs.Referral.Data.EMailServices;

public interface IEmailSender
{
    Task SendEmailAsync(MessageDto messageDto);
}
