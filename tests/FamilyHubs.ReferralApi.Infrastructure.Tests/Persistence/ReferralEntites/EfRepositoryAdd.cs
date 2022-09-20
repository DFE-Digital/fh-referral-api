using AutoFixture;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Events;
using FluentAssertions;

namespace FamilyHubs.ReferralApi.Infrastructure.Tests.Persistence.ReferralEntites;

public class WhenEfRepositoryAdd : BaseEfRepositoryTestFixture
{
    private readonly Fixture _fixture = new Fixture();
    
    [Fact]
    public async Task ThenAddsOrOpensReferral()
    {
        // Arrange
        var referralItem = _fixture.Create<Referral>();     
        ArgumentNullException.ThrowIfNull(referralItem, nameof(referralItem));

        var repository = GetRepository<Referral>();
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));

        // Act
        referralItem.RegisterDomainEvent(new ReferralCreatedEvent(referralItem));
        await repository.AddAsync(referralItem);

        var addedReferralItem = await repository.GetByIdAsync(referralItem.Id);
        ArgumentNullException.ThrowIfNull(addedReferralItem, nameof(addedReferralItem));

        await repository.SaveChangesAsync();

        // Assert
        referralItem.Should().BeEquivalentTo(addedReferralItem);
        string.IsNullOrEmpty(addedReferralItem.Id).Should().BeFalse();
    }
    
}
