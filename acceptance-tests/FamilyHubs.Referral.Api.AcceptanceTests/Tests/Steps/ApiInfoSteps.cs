using FamilyHubs.Referral.Api.AcceptanceTests.Builders.Http;
using FamilyHubs.Referral.Api.AcceptanceTests.Configuration;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Tests.Steps;

public class ApiInfoSteps
{
    readonly string _baseUrl;
    public HttpResponseMessage lastResponse { get; private set; }

    public ApiInfoSteps()
    {
        ConfigModel config = ConfigAccessor.GetApplicationConfiguration();
        _baseUrl = config.BaseUrl;
        lastResponse = new HttpResponseMessage();
    }

    public async Task CheckTheApiInfo()
    {
        lastResponse = await HttpRequestFactory.Get(_baseUrl, "api/info", null, null, null);
    }
}