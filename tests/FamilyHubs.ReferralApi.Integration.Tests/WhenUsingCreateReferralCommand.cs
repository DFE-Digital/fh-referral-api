using AutoMapper;
using Azure.Core;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Data.Entities;
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
            .Include(x => x.UserAccount)
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

        [Fact]
        public async Task ThenCreateReferralAndHardDeleteUser()
        {
            // Arrange
            var newReferral = TestDataProvider.GetReferralDto();

            CreateReferralCommand createCommand = new(newReferral);
            CreateReferralCommandHandler createHandler = new(TestDbContext, Mapper, new Mock<ILogger<CreateReferralCommandHandler>>().Object);

            // First Act
            var handlerResult = await createHandler.Handle(createCommand, new CancellationToken());

            
            // First Assert
            handlerResult.Should().Be(newReferral.Id);
            var referrals = TestDbContext.Referrals.Include(x => x.UserAccount).Where(x => x.UserAccount != null && x.UserAccount.Id == newReferral.ReferralUserAccountDto.Id);
            var userAccount = referrals.Select(x => x.UserAccount).FirstOrDefault(x => x != null && x.Id == newReferral.ReferralUserAccountDto.Id);
            ArgumentNullException.ThrowIfNull(userAccount);
            userAccount.Should().NotBeNull();

            foreach (var referralItem in referrals)
            {
                referralItem.UserAccountId = null; // Detach the UserAccount from the Referral
                referralItem.UserAccount = default!; // Detach the UserAccount from the Referral
            }

            TestDbContext.SaveChanges();

            // Second Act
            TestDbContext.UserAccounts.Remove(userAccount); // Delete the UserAccount (no cascade delete)
            TestDbContext.SaveChanges();
            

            // Assert Final Result
            TestDbContext.UserAccounts.FirstOrDefault(x => x.Id == newReferral.ReferralUserAccountDto.Id).Should().BeNull();
            TestDbContext.Referrals.FirstOrDefault(x => x.Id == newReferral.Id).Should().NotBeNull();
        }


        [Fact]
        public async Task ThenCreateReferralAndHardDeleteOrganisation()
        {
            // Arrange
            var newReferral = TestDataProvider.GetReferralDto();

            CreateReferralCommand createCommand = new(newReferral);
            CreateReferralCommandHandler createHandler = new(TestDbContext, Mapper, new Mock<ILogger<CreateReferralCommandHandler>>().Object);

            // First Act
            var handlerResult = await createHandler.Handle(createCommand, new CancellationToken());


            // First Assert
            handlerResult.Should().Be(newReferral.Id);
            var referrals = TestDbContext.Referrals.Include(x => x.ReferralService).ThenInclude(x => x.Organisation).Where(x => x.ReferralService.Organisation != null && x.ReferralService.Organisation.Id == newReferral.ReferralServiceDto.OrganisationDto.Id);
            var organisation = referrals.Select(x => x.ReferralService.Organisation).FirstOrDefault(x => x != null && x.Id == newReferral.ReferralServiceDto.OrganisationDto.Id);
            ArgumentNullException.ThrowIfNull(organisation);
            organisation.Should().NotBeNull();

            foreach (var referralItem in referrals)
            {
                referralItem.ReferralService.OrganisationId = null; // Detach the Organisation from the Referral
                referralItem.ReferralService.Organisation = default!; // Detach the Organisation from the Referral
            }

            TestDbContext.SaveChanges();

            // Second Act
            TestDbContext.Organisations.Remove(organisation); // Delete the UserAccount (no cascade delete)
            TestDbContext.SaveChanges();


            // Assert Final Result
            TestDbContext.Organisations.FirstOrDefault(x => x.Id == newReferral.ReferralServiceDto.OrganisationDto.Id).Should().BeNull();
            TestDbContext.Referrals.FirstOrDefault(x => x.Id == newReferral.Id).Should().NotBeNull();
        }


    }
}