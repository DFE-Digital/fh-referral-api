using FamilyHubs.Notification.Api.Contracts;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Commands.Notifications;
using FamilyHubs.Referral.Data.Repository;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace FamilyHubs.Referral.UnitTests;

public class WhenSendingNotificationCommand : BaseCreateDbUnitTest
{
    private readonly ApplicationDbContext _mockApplicationDbContext;
    private readonly Mock<INotificationClientService> _notificationClientService;
    private readonly Mock<IConfiguration> _configuration;
    private const string NewRequestTemplateId = "123";
    private const string ClientId = "ClientId";
    private const string BearerTokenSigningKey = "BearerTokenSigningKey";
    private int _emailNotification;
    public WhenSendingNotificationCommand()
    {

        _mockApplicationDbContext = GetApplicationDbContext();
        _mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        _mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        _mockApplicationDbContext.SaveChanges();
        IReadOnlyCollection<Data.Entities.Referral> referrals = ReferralSeedData.SeedReferral();

        foreach (Data.Entities.Referral referral in referrals)
        {
            var status = _mockApplicationDbContext.Statuses.SingleOrDefault(x => x.Name == referral.Status.Name);
            if (status != null)
            {
                referral.Status = status;
            }
        }

        _mockApplicationDbContext.Referrals.AddRange(referrals);
        _mockApplicationDbContext.SaveChanges();

        _notificationClientService = new Mock<INotificationClientService>();
        _configuration = new Mock<IConfiguration>();

        _configuration.Setup(x => x["GovUkOidcConfiguration:Oidc:ClientId"]).Returns(ClientId);
        _configuration.Setup(x => x["GovUkOidcConfiguration:BearerTokenSigningKey"]).Returns(BearerTokenSigningKey);

        _emailNotification = 0;
        _notificationClientService.Setup(x => x.SendNotificationAsync(It.IsAny<MessageDto>(), It.IsAny<JwtSecurityToken>()))
            .Callback(() => _emailNotification++)
            .ReturnsAsync(true);


    }

    [Fact]
    public async Task ThenSendNotification()
    {
        //Arrange
        _configuration.Setup(x => x["ProfessionalSentRequest"]).Returns(NewRequestTemplateId);
        long referralId = ReferralSeedData.SeedReferral().ElementAt(0).Id;
        SendNotificationCommand command = new(referralId);
        SendNotificationCommandHandler handler = new(_mockApplicationDbContext, _notificationClientService.Object, _configuration.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().BeTrue();
        _emailNotification.Should().BeGreaterThan(0);

    }

    [Fact]
    public void ThenSendNotificationFailsAndThrowsException()
    {
        //Arrange
        _configuration.Setup(x => x["ProfessionalSentRequest"]).Returns(NewRequestTemplateId);
        SendNotificationCommand command = new(-1);
        SendNotificationCommandHandler handler = new(_mockApplicationDbContext, _notificationClientService.Object, _configuration.Object);

        //Act
        Func<Task> act = async () => { await handler.Handle(command, new System.Threading.CancellationToken()); };
        
        
        //Assert
        act.Should().ThrowAsync<Exception>();

    }

    [Fact]
    public void ThenSendNotificationFailsAndThrowsExceptionWhenTemplateIdIsInvalid()
    {
        //Arrange
        _configuration.Setup(x => x["ProfessionalSentRequest"]).Returns("");
        SendNotificationCommand command = new(-1);
        SendNotificationCommandHandler handler = new(_mockApplicationDbContext, _notificationClientService.Object, _configuration.Object);

        //Act
        Func<Task> act = async () => { await handler.Handle(command, new System.Threading.CancellationToken()); };


        //Assert
        act.Should().ThrowAsync<Exception>();

    }
}
