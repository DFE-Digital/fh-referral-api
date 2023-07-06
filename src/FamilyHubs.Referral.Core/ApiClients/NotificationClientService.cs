using FamilyHubs.Notification.Api.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FamilyHubs.Referral.Core.ApiClients;

public interface INotificationClientService
{
    Task<bool> SendNotification(MessageDto messageDto, JwtSecurityToken token);
}

public class NotificationClientService : ApiService, INotificationClientService
{
    public NotificationClientService(HttpClient client) : base(client)
    {
    }

    public async Task<bool> SendNotification(MessageDto messageDto, JwtSecurityToken token)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Client.BaseAddress + "api/notify"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(messageDto), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(token)}");

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        bool.TryParse(stringResult, out var result);

        return result;

    }
}
