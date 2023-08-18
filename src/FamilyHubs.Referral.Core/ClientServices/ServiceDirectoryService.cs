using FamilyHubs.ServiceDirectory.Shared.Dto;
using System.Text.Json;

namespace FamilyHubs.Referral.Core.ClientServices;

public interface IServiceDirectoryService
{
    Task<OrganisationDto?> GetOrganisationById(long id);
    Task<ServiceDto?> GetServiceById(long id);
}

public class ServiceDirectoryService : IServiceDirectoryService
{
    private readonly HttpClient _httpClient;

    public ServiceDirectoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OrganisationDto?> GetOrganisationById(long id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_httpClient.BaseAddress + $"api/organisations/{id}")
        };

        using var response = await _httpClient.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentException("Invalid response from ServiceDirectory Api");
        }

        return JsonSerializer.Deserialize<OrganisationDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    }

    public async Task<ServiceDto?> GetServiceById(long id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_httpClient.BaseAddress + $"api/services/{id}")
        };

        using var response = await _httpClient.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentException("Invalid response from ServiceDirectory Api");
        }

        return JsonSerializer.Deserialize<ServiceDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    }
}