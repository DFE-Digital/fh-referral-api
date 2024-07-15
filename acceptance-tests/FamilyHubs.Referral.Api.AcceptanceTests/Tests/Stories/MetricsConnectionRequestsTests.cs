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

    [Theory]
    [InlineData("LaProfessional", 2, HttpStatusCode.OK, HttpStatusCode.NoContent)] // Happy path as LA professional
    [InlineData("LaDualRole", 10, HttpStatusCode.InternalServerError,
        HttpStatusCode.NoContent)] // Happy path as La Dual user
    [InlineData("LaProfessional", 2, 2, HttpStatusCode.BadRequest)] // Invalid statusCode
    [InlineData("LaProfessional", "0", HttpStatusCode.BadRequest,
        HttpStatusCode.BadRequest)] // Invalid connectionRequestId
    [InlineData("LaManager", 2, HttpStatusCode.OK, HttpStatusCode.Forbidden)] // Unauthorised as LA manager
    [InlineData("VcsProfessional", 2, HttpStatusCode.OK, HttpStatusCode.Forbidden)] // Unauthorised as VCS professional
    [InlineData("VcsDualRole", 2, HttpStatusCode.OK, HttpStatusCode.Forbidden)] // Unauthorised as VCS Dual User
    [InlineData("VcsManager", 2, HttpStatusCode.OK, HttpStatusCode.Forbidden)] // Unauthorised as VCS Manager
    [InlineData("DfeAdmin", 2, HttpStatusCode.OK, HttpStatusCode.Forbidden)] // Unauthorised as DfeAdmin
    public void Service_Connection_Metrics_Endpoint_Returns_Expected_Status_Code(string role, long connectionRequestId,
        HttpStatusCode statusCode, HttpStatusCode expectedStatusCode)
    {
        this.Given(s => _sharedSteps.GenerateBearerToken(role))
            .And(s =>
                _steps.GivenIHaveAConnectionMetricsRequest(connectionRequestId, statusCode))
            .When(s => _steps.WhenISendARequest(_sharedSteps.bearerToken))
            .Then(s => _sharedSteps.VerifyStatusCode(_steps.lastResponse, expectedStatusCode))
            .BDDfy();
    }

    //Add all tests that make up the story to this class.
    [Theory]
    [InlineData(2, HttpStatusCode.OK, HttpStatusCode.Unauthorized)] // Happy path as LA professional
    public void Service_Connection_Metrics_Endpoint_Errors_Invalid_Bearer_Token(long connectionRequestId,
        HttpStatusCode statusCode, HttpStatusCode expectedStatusCode)
    {
        this.Given(s => _sharedSteps.HaveAnInvalidBearerToken())
            .And(s =>
                _steps.GivenIHaveAConnectionMetricsRequest(connectionRequestId, statusCode))
            .When(s => _steps.WhenISendARequest(_sharedSteps.bearerToken))
            .Then(s => _sharedSteps.VerifyStatusCode(_steps.lastResponse, expectedStatusCode))
            .BDDfy();
    }
}