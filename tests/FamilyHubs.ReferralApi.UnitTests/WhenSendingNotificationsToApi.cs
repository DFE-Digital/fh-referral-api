using FamilyHubs.Notification.Api.Contracts;
using FamilyHubs.Referral.Core.ApiClients;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moq.Protected;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace FamilyHubs.Referral.UnitTests;


public class WhenSendingNotificationsToApi
{
    private const string NewRequestTemplateId = "123";
    private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;
    private readonly NotificationClientService notificationClientService;
    private readonly JwtSecurityToken token;
    private readonly MessageDto messageDto;

    public WhenSendingNotificationsToApi()
    {
        messageDto = new MessageDto
        {
            ApiKeyType = ApiKeyType.ConnectKey,
            NotificationEmails = new List<string> { "someone@email.com" },
            TemplateId = NewRequestTemplateId,
            TemplateTokens = new Dictionary<string, string>
            {
                { "reference number", "0001" },
                { "RequestNumber", "0001" },
                { "ServiceName", "ServiceName" },
                { "ViewConnectionRequestUrl",  "wwww.someurl.com"},
                { "Name of service", "Special Test Service" },
                { "link to specific connection request", "wwww.someurl.com" }
            }
        };

        var jti = Guid.NewGuid().ToString();
        var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes("BearerTokenSigningKey"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        token = new JwtSecurityToken(
            claims: new List<Claim>
               {
                    new Claim("sub", "ClientId"),
                    new Claim("jti", jti),
                    new Claim(ClaimTypes.Role, "Professional")

               },
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(5)
            );

        httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        httpClient.BaseAddress = new Uri("https://example.com");
        notificationClientService = new NotificationClientService(httpClient, new Mock<ILogger<NotificationClientService>>().Object);

    }

    [Fact]
    public async Task ThenSendNotificationAsync_ValidRequest_ReturnsTrue()
    {
        // Arrange
        var expectedStatusCode = HttpStatusCode.OK;

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = expectedStatusCode,
            Content = new StringContent("true")
        };

        int callback = 0;

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback(() => callback++)
            .ReturnsAsync(responseMessage);

        // Act
        var result = await notificationClientService.SendNotificationAsync(messageDto, token);

        // Assert
        result.Should().BeTrue();
        callback.Should().Be(1);

    }

    [Fact]
    public async Task ThenSendNotificationAsync_InvalidRequest_ReturnsFalse()
    {
        var expectedStatusCode = HttpStatusCode.BadRequest;

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = expectedStatusCode,
            Content = new StringContent("false")
        };

        int callback = 0;

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback(() => callback++)
            .ReturnsAsync(responseMessage);

        // Act
        var result = await notificationClientService.SendNotificationAsync(messageDto, token);

        // Assert
        result.Should().Be(false);
        callback.Should().Be(1);
        

    }


}
