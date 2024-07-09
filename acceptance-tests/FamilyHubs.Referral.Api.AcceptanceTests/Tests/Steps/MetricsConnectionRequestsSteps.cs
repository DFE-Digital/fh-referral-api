using FluentAssertions;
using System.Net;
using FamilyHubs.Referral.Api.AcceptanceTests.Builders.Http;
using FamilyHubs.Referral.Api.AcceptanceTests.Configuration;
using FamilyHubs.Referral.Api.AcceptanceTests.Models;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Tests.Steps;

/// <summary>
/// These are the steps required for testing the Postcodes endpoints
/// </summary>
public class MetricsConnectionRequestsSteps
{
    private readonly string _baseUrl;
    private ConnectionRequest _request;
    private HttpStatusCode _statusCode;
    public HttpResponseMessage lastResponse { get; private set; }
    public MetricsConnectionRequestsSteps()
    {
        _baseUrl = ConfigAccessor.GetApplicationConfiguration().BaseUrl;
        lastResponse = new HttpResponseMessage();
    }

    private static string ResponseNotExpectedMessage(HttpMethod method, System.Uri requestUri,
        HttpStatusCode statusCode)
    {
        return $"Response from {method} {requestUri} {statusCode} was not as expected";
    }

    #region Step Definitions

    #region Given

    public void GivenIHaveAConnectionMetricsRequest(int connectionRequestId, int statusCode)
    {
        DateTime time = DateTime.UtcNow;
        _request = new ConnectionRequest()
        {
            connectionRequestId = connectionRequestId,
            httpResponseCode = statusCode,
            requestTimestamp = time,
        };
    }

    #endregion Given

    #region When

    public async Task<HttpStatusCode> WhenISendARequest(string bearerToken)
    {
        lastResponse = await HttpRequestFactory.Put(_baseUrl, "api/metrics/connection-request", _request, bearerToken,
            null, null);
        _statusCode = lastResponse.StatusCode;

        return _statusCode;
    }

    #endregion When

    #endregion Step Definitions
}