using AutoMapper;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.Commands.CreateOrUpdateOrganisation;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.Referral.UnitTests;

public class WhenUsingCreateOrUpdateOrganisationCommand : BaseCreateDbUnitTest
{
    [Fact]
    public async Task ThenCreateNewOrganisation()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateOrUpdateOrganisationCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        var organisation = new OrganisationDto
        {
            Id = 4,
            Name = "Test Organisation",
            Description = "Test Organisation Description",
        };
        CreateOrUpdateOrganisationCommand command = new(organisation);
        CreateOrUpdateOrganisationCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().BeGreaterThan(0);
        result.Should().Be(organisation.Id);
    }

    [Fact]
    public async Task ThenUpdateOrganisation()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateOrUpdateOrganisationCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Organisations.Add(new Data.Entities.Organisation
        {
            Id = 4,
            Name = "Test Organisation",
            Description = "Test Organisation Description",
        });
        mockApplicationDbContext.SaveChanges();
        var expected = new OrganisationDto
        {
            Id = 4,
            Name = "Test Organisation - Updated",
            Description = "Test Organisation Description - Updated",
        };
        CreateOrUpdateOrganisationCommand command = new(expected);
        CreateOrUpdateOrganisationCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());
        var dbOrganisation = mockApplicationDbContext.Organisations.FirstOrDefault(x => x.Id == 4);

        //Assert
        result.Should().BeGreaterThan(0);
        result.Should().Be(expected.Id);
        ArgumentNullException.ThrowIfNull(dbOrganisation);
        dbOrganisation.Name.Should().Be(expected.Name);
        dbOrganisation.Description.Should().Be(expected.Description);

    }
}
