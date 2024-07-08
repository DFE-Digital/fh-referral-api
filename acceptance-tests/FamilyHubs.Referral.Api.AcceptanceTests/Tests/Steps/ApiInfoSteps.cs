using FamilyHubs.Referral.Api.AcceptanceTests.Builders.Http;
using FamilyHubs.Referral.Api.AcceptanceTests.Configuration;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Tests.Steps;

public class ApiInfoSteps
{
    readonly ConfigModel config;
    readonly string baseUrl;
    public HttpResponseMessage lastResponse { get; private set; }

    public ApiInfoSteps()
    {
        config = ConfigAccessor.GetApplicationConfiguration();
        baseUrl = config.BaseUrl;
        lastResponse = new HttpResponseMessage();
    }
    public async Task ICheckTheApiInfo()
    {
        lastResponse = await HttpRequestFactory.Get(baseUrl, "api/info", null, null, null);
    }
}