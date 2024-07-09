using System.Net;
using FamilyHubs.Referral.Api.AcceptanceTests.Tests.Steps;
using TestStack.BDDfy;
using Xunit;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Tests.Stories;

// Define the story/feature being tested
[Story(
    AsA = "user of the metrics api",
    IWant = "to be able to update usage information for connection requests made",
    SoThat = "I can record how search functionality is being used")]
[TestClass]
public class MetricsConnectionRequestsTests
{
    private readonly MetricsConnectionRequestsSteps _steps;
    private readonly SharedSteps _sharedSteps;

    public MetricsConnectionRequestsTests()
    {
        _steps = new MetricsConnectionRequestsSteps();
        _sharedSteps = new SharedSteps();
    }

    //Add all tests that make up the story to this class.
    [Theory]
    [InlineData("LaProfessional", 2, 200, HttpStatusCode.OK)] // Happy path as LA professional
    [InlineData("LaDualUser", 10, 500, HttpStatusCode.OK)] // Happy path as La Dual user
    [InlineData("LaProfessional", 2, 2, HttpStatusCode.BadRequest)] // Invalid statusCode
    [InlineData("LaProfessional", '0', 400, HttpStatusCode.BadRequest)] // Invalid connectionRequestId
    [InlineData("LaManager", 2, 200, HttpStatusCode.Forbidden)] // Unauthorised as LA manager
    [InlineData("VcsProfessional", 2, 200, HttpStatusCode.Forbidden)] // Unauthorised as VCS professional
    [InlineData("VcsDualUser", 2, 200, HttpStatusCode.Forbidden)] // Unauthorised as VCS Dual User
    [InlineData("VcsManager", 2, 200, HttpStatusCode.Forbidden)] // Unauthorised as VCS Manager
    [InlineData("DfeAdmin", 2, 200, HttpStatusCode.Forbidden)] // Unauthorised as DfeAdmin
    public void Service_Connection_Metrics_Endpoint_Returns_Expected_Status_Code(string role, int connectionRequestId,
        int statusCode, HttpStatusCode expectedStatusCode)
    {
        this.Given(s => _sharedSteps.GenerateBearerToken(role))
            .And(s =>
                _steps.GivenIHaveAConnectionMetricsRequest(connectionRequestId, statusCode))
            .When(s => _steps.WhenISendARequest(_sharedSteps.bearerToken))
            .Then(s => _sharedSteps.VerifyStatusCode(_steps.lastResponse, expectedStatusCode))
            .BDDfy();
    }
}