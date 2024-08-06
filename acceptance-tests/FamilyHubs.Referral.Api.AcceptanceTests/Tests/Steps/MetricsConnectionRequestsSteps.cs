using FamilyHubs.ReferralService.Shared.Dto;
using System.Net;
using FamilyHubs.Referral.Api.AcceptanceTests.Builders.Http;
using FamilyHubs.Referral.Api.AcceptanceTests.Configuration;
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Tests.Steps;

/// <summary>
/// These are the steps required for testing the Postcodes endpoints
/// </summary>
public class MetricsConnectionRequestsSteps
{
    private readonly string _baseUrl;
    private CreateReferralDto _request;
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

    public void GivenIHaveAReferralsRequest()
    {
        _request = GetCreateReferralDto();
    }

    #endregion Given

    #region When

    public async Task<HttpStatusCode> WhenISendARequest(string bearerToken)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>() { };
        headers.Add("traceparent", new Guid().ToString());
        
        lastResponse = await HttpRequestFactory.Post(_baseUrl, "api/referrals", _request, bearerToken,
            headers, null);
        _statusCode = lastResponse.StatusCode;

        return _statusCode;
    }

    #endregion When

    #endregion Step Definitions

    private static CreateReferralDto GetCreateReferralDto()
    {
        return new CreateReferralDto(GetReferralDto(), new ConnectionRequestsSentMetricDto(DateTimeOffset.UtcNow));
    }

    private static ReferralDto GetReferralDto()
    {
        return new ReferralDto
        {
            ReasonForSupport = "Reason For Support",
            EngageWithFamily = "Engage With Family",
            RecipientDto = new RecipientDto
            {
                Id = 2,
                Name = "Joe Blogs",
                Email = "JoeBlog@email.com",
                Telephone = "078123456",
                TextPhone = "078123456",
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                TownOrCity = "Town or City",
                County = "County",
                PostCode = "B30 2TV"
            },
            ReferralUserAccountDto = new UserAccountDto
            {
                Id = 2,
                EmailAddress = "Bob.Referrer@email.com",
                Name = "Bob Referrer",
                PhoneNumber = "1234567890",
                Team = "Team",
                UserAccountRoles = new List<UserAccountRoleDto>()
                    {
                        new UserAccountRoleDto
                        {
                            UserAccount = new UserAccountDto
                            {
                                EmailAddress = "Bob.Referrer@email.com",
                            },
                            Role = new RoleDto
                            {
                                Name = "LaProfessional"
                            }
                        }
                    },
                ServiceUserAccounts = new List<UserAccountServiceDto>(),
                OrganisationUserAccounts = new List<UserAccountOrganisationDto>(),
            },
            Status = new ReferralStatusDto
            {
                Id = 1,
                Name = "New",
                SortOrder = 0
            },
            ReferralServiceDto = new ReferralServiceDto
            {
                Id = 809,
                Name = "Elop Mentoring",
                Description = "Elop Mentoring",
                Url = "www.service.com",
                OrganisationDto = new OrganisationDto
                {
                    Id = 237,
                    ReferralServiceId = 809,
                    Name = "Elop Mentoring",
                    Description = "Elop Mentoring",
                }
            }
        };
    }
}