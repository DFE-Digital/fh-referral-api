using AutoFixture;
using FamilyHubs.ReferralApi.Data.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Infrastructure.Tests.Persistence.ReferralEntites;

public class WhenEfRepositoryUpdate : BaseEfRepositoryTestFixture
{
    private readonly Fixture _fixture = new Fixture();

    //[Fact]
    //public async Task ThenUpdatesOpenReferralOrganisationAfterAddingIt()
    //{
    //    // Arrange
    //    var referralItem = _fixture.Create<Referral>();
    //    ArgumentNullException.ThrowIfNull(referralItem);

    //    var repository = GetRepository<Referral>();
    //    ArgumentNullException.ThrowIfNull(repository);
    //    await repository.AddAsync(referralItem);

    //    DbContext.Entry(referralItem).State = EntityState.Detached;             // detach the item so we get a different instance

    //    var addedReferral = await repository.GetByIdAsync(referralItem.Id); // fetch the OpenReferralOrganisation and update its name
    //    if (addedReferral == null)
    //    {
    //        addedReferral.Should().NotBeNull();
    //        return;
    //    }

    //    // Act
    //    addedReferral.ServiceName = "Brum1 Council";
    //    referralItem.Should().NotBeEquivalentTo(addedReferral);
    //    await repository.UpdateAsync(addedReferral);

    //    var updatedOpenReferralOrganisation = await repository.GetByIdAsync(addedReferral.Id);

    //    // Assert
    //    updatedOpenReferralOrganisation.Should().NotBeNull();
    //    ArgumentNullException.ThrowIfNull(referralItem);
    //    referralItem.ServiceName.Should().NotBeSameAs(updatedOpenReferralOrganisation?.ServiceName);
    //    referralItem.Id.Should().BeEquivalentTo(updatedOpenReferralOrganisation?.Id);
    //}
}
