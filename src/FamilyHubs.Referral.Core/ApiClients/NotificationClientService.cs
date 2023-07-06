using FamilyHubs.Notification.Api.Contracts;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FamilyHubs.Referral.Core.ApiClients;

public interface INotificationClientService
{
    Task<bool> SendNotificationAsync(MessageDto messageDto, JwtSecurityToken token);
}

public class NotificationClientService : ApiService, INotificationClientService
{
    private readonly ILogger<NotificationClientService> _logger;
    public NotificationClientService(HttpClient client, ILogger<NotificationClientService> logger) : base(client)
    {
        _logger = logger;
    }

    public async Task<bool> SendNotificationAsync(MessageDto messageDto, JwtSecurityToken token)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Client.BaseAddress + "api/notify"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(messageDto), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(token)}");

        using var response = await Client.SendAsync(request);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex) 
        { 
            _logger.LogError(ex, ex.Message);
            return false;
        }

        var stringResult = await response.Content.ReadAsStringAsync();
        bool.TryParse(stringResult, out var result);

        return result;

    }
}
