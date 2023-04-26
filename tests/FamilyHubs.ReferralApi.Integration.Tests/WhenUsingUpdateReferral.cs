using AutoMapper;
using FamilyHubs.ReferralApi.Core.Commands.UpdateReferral;
using FamilyHubs.ReferralApi.Data.Entities;
using FamilyHubs.ReferralCommon.Shared.Dto;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.ReferralApi.Integration.Tests;

public class WhenUsingUpdateReferral : DataIntegrationTestBase
{
    [Fact]
    public async Task ThenUpdateReferralOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();
        referral.ReferenceNumber = "Unit Test Ref";
        referral.ReasonForSupport = "Unit Test Reason For Support";
        referral.EngageWithFamily = "Unit Test Engage With Family";

        UpdateReferralCommand command = new(referral.Id, referral);
        UpdateReferralCommandHandler handler = new(TestDbContext, Mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBe(0);
        result.Should().Be(referral.Id);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();
        actualService!.ReferenceNumber.Should().Be(referral.ReferenceNumber);

    }
    public async Task<ReferralDto> CreateReferral(ReferralDto? newReferral = null)
    {
        
        var referral = Mapper.Map<Referral>(newReferral ?? TestDataProvider.GetReferralDto());

        TestDbContext.Referrals.Add(referral);

        await TestDbContext.SaveChangesAsync();

        return Mapper.Map(referral, newReferral ?? TestDataProvider.GetReferralDto());
    }
}
