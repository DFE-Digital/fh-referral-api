using FamilyHubs.Referral.Data.EMailServices;
using FamilyHubs.ReferralService.Shared.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Notify.Interfaces;

namespace FamilyHubs.Referral.UnitTests;

public class WhenUsingGovNotifySender
{
    [Fact]
    public async Task ThenSendNotification()
    {
        //Arrange
        IOptions<GovNotifySetting> mockGovSettings = Options.Create<GovNotifySetting>(new GovNotifySetting
        {
            APIKey = "APIKey",
            TemplateId = "TemplateId"
        });

        Mock<IAsyncNotificationClient> mockAsyncNotificationClient = new Mock<IAsyncNotificationClient>();
        int sendEmailCallback = 0;
        mockAsyncNotificationClient.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback(() => sendEmailCallback++);

        GovNotifySender govNotifySender = new GovNotifySender(mockAsyncNotificationClient.Object, mockGovSettings);
        var dict = new Dictionary<string, string>();
        dict.Add("Key1", "Value1");
        dict.Add("Key2", "Value2");

        MessageDto messageDto = new MessageDto
        {
            RecipientEmail = "someone@email.com",
            TemplateId = Guid.NewGuid().ToString(),
            TemplateTokens = dict
        };

        //Act
        await govNotifySender.SendEmailAsync(messageDto);


        //Assert
        sendEmailCallback.Should().Be(1);


    }
}
