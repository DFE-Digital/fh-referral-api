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

            CreateReferralCommand createCommand = new(newReferral);
            CreateReferralCommandHandler createHandler = new(TestDbContext, Mapper, new Mock<ILogger<CreateReferralCommandHandler>>().Object);


            //Act
            var result = await createHandler.Handle(createCommand, new CancellationToken());

            //Assert
            result.Should().NotBe(0);
            var actualService = TestDbContext.Referrals
                .Include(x => x.Status)
            .Include(x => x.ReferralUserAccount)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.Organisation)
            .AsNoTracking()
            .SingleOrDefault(s => s.Id == result);
            actualService.Should().NotBeNull();

            var actualReferral = Mapper.Map<ReferralDto>(actualService);

            actualReferral.Should().BeEquivalentTo(newReferral, options =>
                options.Excluding((IMemberInfo info) => info.Name.Contains("Id"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("Created"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("CreatedBy"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("LastModified"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("LastModifiedBy"))
                    );
        }
    }
}