using FamilyHubs.Referral.Core.ClientServices;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using FamilyHubs.ReferralService.Shared.CreateUpdateDto;
using Microsoft.ApplicationInsights.Extensibility.W3C;

namespace FamilyHubs.Referral.Integration.Tests
{
    //todo: unit test to check metrics are created

    public class WhenUsingCreateReferralCommand : DataIntegrationTestBase
    {
        [Fact]
        public async Task ThenCreateReferral()
        {
            const long userOrganisationId = 123L;
            var newReferral = new CreateReferralDto(TestDataProvider.GetReferralDto(),
                new ConnectionRequestsSentMetricDto(userOrganisationId));
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

            CreateReferralCommand createCommand = new(newReferral);
            CreateReferralCommandHandler createHandler = new(TestDbContext, Mapper, mockServiceDirectory.Object,
                new Mock<ILogger<CreateReferralCommandHandler>>().Object);

            var activity = new Activity("TestActivity");
            activity.SetParentId(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom());
            activity.Start();
            Activity.Current = activity;

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
            const long userOrganisationId = 123L;

            var newReferral = new CreateReferralDto(TestDataProvider.GetReferralDto(),
                new ConnectionRequestsSentMetricDto(userOrganisationId));
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

            CreateReferralCommand createCommand = new(newReferral);
            CreateReferralCommandHandler createHandler = new(TestDbContext, Mapper, mockServiceDirectory.Object,
                new Mock<ILogger<CreateReferralCommandHandler>>().Object);

            var activity = new Activity("TestActivity");
            activity.SetParentId(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom());
            activity.Start();
            Activity.Current = activity;
            string expectedRequestCorrelationId = Activity.Current!.TraceId.ToString();

            //Act
            var result = await createHandler.Handle(createCommand, new CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            //var metric =
            //    TestDbContext.ConnectionRequestsSentMetric.SingleOrDefault(m => m.ConnectionRequestId == result.Id);
            var metric = TestDbContext.ConnectionRequestsSentMetric.SingleOrDefault();

            metric.Should().NotBeNull();
            metric!.RequestCorrelationId.Should().Be(expectedRequestCorrelationId);
        }
    }
}