using FamilyHubs.Referral.Core.ClientServices;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.Referral.Integration.Tests
{
    public class WhenUsingCreateReferralCommand : DataIntegrationTestBase
    {
        [Fact]
        public async Task ThenCreateReferral()
        {
            var newReferral = TestDataProvider.GetReferralDto();
            Mock<IServiceDirectoryService> mockServiceDirectory = new Mock<IServiceDirectoryService>();
            mockServiceDirectory.Setup(x => x.GetOrganisationById(It.IsAny<long>())).ReturnsAsync(new ServiceDirectory.Shared.Dto.OrganisationDto
            {
                Id = 2,
                Name = "Organisation",
                Description = "Organisation Description",
                OrganisationType = ServiceDirectory.Shared.Enums.OrganisationType.VCFS,
                AdminAreaCode = "AdminAreaCode"
            });

            mockServiceDirectory.Setup(x => x.GetServiceById(It.IsAny<long>())).ReturnsAsync(new ServiceDirectory.Shared.Dto.ServiceDto
            {
                Id = 2,
                Name = "Service",
                Description = "Service Description",
                ServiceOwnerReferenceId = "ServiceOwnerReferenceId",
                ServiceType = ServiceDirectory.Shared.Enums.ServiceType.FamilyExperience
            });

            CreateReferralCommand createCommand = new(newReferral);
            CreateReferralCommandHandler createHandler = new(TestDbContext, Mapper, mockServiceDirectory.Object, new Mock<ILogger<CreateReferralCommandHandler>>().Object);


            //Act
            var result = await createHandler.Handle(createCommand, new CancellationToken());

            //Assert
            result.Should().NotBe(0);
            var actualService = TestDbContext.Referrals
                .Include(x => x.Status)
            .Include(x => x.UserAccount)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.Organisation)
            .AsNoTracking()
            .SingleOrDefault(s => s.Id == result.Id);
            actualService.Should().NotBeNull();

            var actualReferral = Mapper.Map<ReferralDto>(actualService);

            actualReferral.Should().BeEquivalentTo(newReferral, options =>
                options.Excluding((IMemberInfo info) => info.Name.Contains("Id"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("Created"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("CreatedBy"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("LastModified"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("LastModifiedBy"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("Url"))
                    );
        }
    }
}