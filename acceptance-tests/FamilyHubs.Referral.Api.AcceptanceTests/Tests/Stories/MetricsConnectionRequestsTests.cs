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
    [InlineData("LaProfessional", HttpStatusCode.OK)] // Happy path as LA professional
    [InlineData("LaDualRole", HttpStatusCode.OK)] // Happy path as La Dual user
    [InlineData("LaManager", HttpStatusCode.Forbidden)] // Unauthorised as LA manager
    [InlineData("VcsProfessional", HttpStatusCode.Forbidden)] // Unauthorised as VCS professional
    [InlineData("VcsDualRole", HttpStatusCode.Forbidden)] // Unauthorised as VCS Dual User
    [InlineData("VcsManager", HttpStatusCode.Forbidden)] // Unauthorised as VCS Manager
    [InlineData("DfeAdmin", HttpStatusCode.Forbidden)] // Unauthorised as DfeAdmin
    public void Connection_Request_Returns_Expected_Status_Code(string role, HttpStatusCode expectedStatusCode)
    {
        this.Given(s => _sharedSteps.GenerateBearerToken(role))
            .And(s => _steps.GivenIHaveAReferralsRequest())
            .When(s => _steps.WhenISendARequest(_sharedSteps.bearerToken))
            .Then(s => _sharedSteps.VerifyStatusCode(_steps.lastResponse, expectedStatusCode))
            .BDDfy();
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    public void Connection_Request_Endpoint_Errors_Invalid_Bearer_Token(HttpStatusCode expectedStatusCode)
    {
        this.Given(s => _sharedSteps.HaveAnInvalidBearerToken())
            .And(s => _steps.GivenIHaveAReferralsRequest())
            .When(s => _steps.WhenISendARequest(_sharedSteps.bearerToken))
            .Then(s => _sharedSteps.VerifyStatusCode(_steps.lastResponse, expectedStatusCode))
            .BDDfy();
    }
}