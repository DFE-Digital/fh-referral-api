using FamilyHubs.ReferralApi.Api;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.ReferralApi.Infrastructure.Tests;

public  class WhenUsingDatabaseContextFactory
{
    [Fact]
    public void ThenApplicationDbContextIsReturned()
    {
        //Arrange
        DatabaseContextFactory databaseContextFactory = new DatabaseContextFactory();
        string[] args = Array.Empty<string>();

        //Act
        var result = databaseContextFactory.CreateDbContext(args);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationDbContext>();

    }
}
