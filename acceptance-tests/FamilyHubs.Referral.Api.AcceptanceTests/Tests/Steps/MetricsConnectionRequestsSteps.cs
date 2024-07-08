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
  private HttpResponseMessage _lastResponse;
  private HttpStatusCode _statusCode;
  private const string connectionRequestPath = "api/metrics/connection-request";
  public MetricsConnectionRequestsSteps()
  {
      _baseUrl = ConfigAccessor.GetApplicationConfiguration().BaseUrl;
  }
  private static string ResponseNotExpectedMessage(HttpMethod method, System.Uri requestUri, HttpStatusCode statusCode)
  {
      return $"Response from {method} {requestUri} {statusCode} was not as expected";
  }
  #region Step Definitions

  #region Given
  
  public void GivenIHaveASearchServiceRequest(int connectionRequestId, int statusCode)
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
  public async Task<HttpStatusCode> WhenISendARequest()
  {
      _lastResponse = await HttpRequestFactory.Put(_baseUrl, connectionRequestPath, _request, _bearerToken, null, null);
      _statusCode = _lastResponse.StatusCode;
    
      return _statusCode;
  }

  #endregion When

  #endregion Step Definitions
}
