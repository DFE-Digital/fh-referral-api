using System.Net;
using FamilyHubs.Referral.Api.AcceptanceTests.Tests.Steps;
using TestStack.BDDfy;
using Xunit;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Tests.Stories;

// Define the story/feature being tested
[Story(
    AsA = "user of the metrics api",
    IWant = "to be able to update usage information for postcode searches and filters",
    SoThat = "I can record how search functionality is being used")]
[TestClass]
public class MetricsConnectionRequestsTests
{
    private readonly MetricsConnectionRequestsSteps _steps;

    public MetricsConnectionRequestsTests()
    {
        //Get instances of the steps required for the test
        _steps = new MetricsConnectionRequestsSteps();
    }

    //Add all tests that make up the story to this class.
    [Theory]
    [InlineData("20", "E1 2EN", "200", "1", "2", HttpStatusCode.OK)] // As a Find user
    public void Service_Search_Metrics_Endpoint_Returns_A_Status_Code_Ok(
        string radius, string postcode, string statusCode, string searchTriggerEventId, string serviceSearchTypeId,
        HttpStatusCode expectedStatusCode)
    {
        this.Given(s =>
                _steps.GivenIHaveASearchServiceRequest(radius, postcode, statusCode, searchTriggerEventId,
                    serviceSearchTypeId))
            .When(s => _steps.WhenISendARequest())
            .Then(s => _steps.ThenExpectedStatusCodeReturned(expectedStatusCode))
            .BDDfy();
    }
    

    
    //Negative Scenarios
    [Theory]
   public void Service_Search_Metrics_Endpoint_Returns_Status_Code_Internal_Server_Error(string radius, string postcode, string statusCode,
        string searchTriggerEventId, string serviceSearchTypeId, HttpStatusCode expectedStatusCode)
    {
        this.Given(s =>
                _steps.GivenIHaveASearchServiceRequest(radius, postcode, statusCode, searchTriggerEventId,
                    serviceSearchTypeId))
            .When(s => _steps.WhenISendARequest())
            .Then(s => _steps.ThenExpectedStatusCodeReturned(expectedStatusCode))
            .BDDfy();
    }
}