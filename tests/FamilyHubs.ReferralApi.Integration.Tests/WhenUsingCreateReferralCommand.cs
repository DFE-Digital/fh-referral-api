using AutoMapper;
using FamilyHubs.ReferralApi.Core.Commands.CreateReferral;
using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FamilyHubs.ReferralApi.Integration.Tests
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
            .Include(x => x.Referrer)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.ReferralOrganisation)
            .AsNoTracking()
            .SingleOrDefault(s => s.Id == result);
            actualService.Should().NotBeNull();

            var actualReferral = Mapper.Map<ReferralDto>(actualService);

            actualReferral.Should().BeEquivalentTo(newReferral, options =>
                options.Excluding((IMemberInfo info) => info.Name.Contains("Id"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("CreatedBy"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("CreatedBy"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("LastModified"))
                       .Excluding((IMemberInfo info) => info.Name.Contains("LastModifiedBy"))
                    );
        }
    }
}