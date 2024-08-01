using FamilyHubs.Referral.Core.ClientServices;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using System.Net;
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.SharedKernel.Identity.Models;

namespace FamilyHubs.Referral.Integration.Tests;

public class WhenUsingCreateReferralCommand : DataIntegrationTestBase
{
    public DateTimeOffset RequestTimestamp { get; set; }
    public FamilyHubsUser FamilyHubsUser { get; set; }
    public const long ExpectedAccountId = 123L;
    public const long ExpectedOrganisationId = 456L;
    private const long ExpectedVcsOrganisationId = 2L;
    public string ExpectedRequestCorrelationId { get; set; }

    public WhenUsingCreateReferralCommand()
    {
        RequestTimestamp = new DateTimeOffset(new DateTime(2025, 1, 1, 12, 0, 0));

        FamilyHubsUser = new FamilyHubsUser
        {
            AccountId = ExpectedAccountId.ToString(),
            OrganisationId = ExpectedOrganisationId.ToString()
        };

        var activity = new Activity("TestActivity");
        activity.SetParentId(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom());
        activity.Start();
        Activity.Current = activity;
        ExpectedRequestCorrelationId = Activity.Current!.TraceId.ToString();
    }

    [Fact]
    public async Task ThenCreateReferral()
    {
        var newReferral = new CreateReferralDto(TestDataProvider.GetReferralDto(),
            new ConnectionRequestsSentMetricDto(RequestTimestamp));
        Mock<IServiceDirectoryService> mockServiceDirectory = new Mock<IServiceDirectoryService>();
        mockServiceDirectory.Setup(x => x.GetOrganisationById(It.IsAny<long>())).ReturnsAsync(
            new ServiceDirectory.Shared.Dto.OrganisationDto
            {
                Id = 2,
                Name = "Organisation",
                Description = "Organisation Description",
                OrganisationType = ServiceDirectory.Shared.Enums.OrganisationType.VCFS,
                AdminAreaCode = "AdminAreaCode"
            });

        mockServiceDirectory.Setup(x => x.GetServiceById(It.IsAny<long>())).ReturnsAsync(
            new ServiceDirectory.Shared.Dto.ServiceDto
            {
                Id = 2,
                Name = "Service",
                Description = "Service Description",
                ServiceType = ServiceDirectory.Shared.Enums.ServiceType.FamilyExperience
            });

        CreateReferralCommand createCommand = new(newReferral, FamilyHubsUser);
        CreateReferralCommandHandler createHandler = new(TestDbContext, Mapper, mockServiceDirectory.Object,
            new Mock<ILogger<CreateReferralCommandHandler>>().Object);

        //Act
        var result = await createHandler.Handle(createCommand, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
        var actualService = TestDbContext.Referrals
            .Include(x => x.Status)
            .Include(x => x.UserAccount)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.Organisation)
            .AsNoTracking()
            .SingleOrDefault(s => s.Id == result.Id);
        actualService.Should().NotBeNull();

        //todo: it's a bad idea to use the same code as the unit test uses to create the expected object
        // it effectively removes the mapping from the test, as if it's wrong, the test will still pass
        var actualReferral = Mapper.Map<ReferralDto>(actualService);

        actualReferral.Should().BeEquivalentTo(newReferral.Referral, options =>
            options.Excluding((IMemberInfo info) => info.Name.Contains("Id"))
                .Excluding((IMemberInfo info) => info.Name.Contains("Created"))
                .Excluding((IMemberInfo info) => info.Name.Contains("CreatedBy"))
                .Excluding((IMemberInfo info) => info.Name.Contains("LastModified"))
                .Excluding((IMemberInfo info) => info.Name.Contains("LastModifiedBy"))
                .Excluding((IMemberInfo info) => info.Name.Contains("Url"))
        );
    }

    [Fact]
    public async Task ThenCreateReferralCreatesConnectionRequestsSentMetric()
    {
        var newReferral = new CreateReferralDto(TestDataProvider.GetReferralDto(),
            new ConnectionRequestsSentMetricDto(RequestTimestamp));
        Mock<IServiceDirectoryService> mockServiceDirectory = new Mock<IServiceDirectoryService>();
        mockServiceDirectory.Setup(x => x.GetOrganisationById(It.IsAny<long>())).ReturnsAsync(
            new ServiceDirectory.Shared.Dto.OrganisationDto
            {
                Id = 2,
                Name = "Organisation",
                Description = "Organisation Description",
                OrganisationType = ServiceDirectory.Shared.Enums.OrganisationType.VCFS,
                AdminAreaCode = "AdminAreaCode"
            });

        mockServiceDirectory.Setup(x => x.GetServiceById(It.IsAny<long>())).ReturnsAsync(
            new ServiceDirectory.Shared.Dto.ServiceDto
            {
                Id = 2,
                Name = "Service",
                Description = "Service Description",
                ServiceType = ServiceDirectory.Shared.Enums.ServiceType.FamilyExperience
            });

        CreateReferralCommand createCommand = new(newReferral, FamilyHubsUser);
        CreateReferralCommandHandler createHandler = new(TestDbContext, Mapper, mockServiceDirectory.Object,
            new Mock<ILogger<CreateReferralCommandHandler>>().Object);

        //Act
        var result = await createHandler.Handle(createCommand, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);

        string expectedConnectionReferenceCode = result.Id.ToString("X6");

        var metric = TestDbContext.ConnectionRequestsSentMetric.SingleOrDefault();

        metric.Should().NotBeNull();

        metric!.RequestCorrelationId.Should().Be(ExpectedRequestCorrelationId);
        metric.UserAccountId.Should().Be(ExpectedAccountId);
        metric.LaOrganisationId.Should().Be(ExpectedOrganisationId);
        metric.VcsOrganisationId.Should().Be(ExpectedVcsOrganisationId);
        metric.RequestTimestamp.Should().Be(RequestTimestamp.DateTime);
        metric.ResponseTimestamp.Should().NotBeNull();
        metric.HttpResponseCode.Should().Be(HttpStatusCode.OK);
        metric.ConnectionRequestId.Should().Be(result.Id);
        metric.ConnectionRequestReferenceCode.Should().Be(expectedConnectionReferenceCode);
    }
}
