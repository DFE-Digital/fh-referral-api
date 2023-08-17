using AutoMapper;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.Commands.CreateOrUpdateService;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.Referral.UnitTests;

public class WhenUsingCreateOrUpdateServiceCommand : BaseCreateDbUnitTest
{
    [Fact]
    public async Task ThenCreateNewService()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateOrUpdateServiceCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        var service = new ReferralServiceDto
        {
            Id = 4,
            Name = "Test Service",
            Description = "Test Organisation Service",
            OrganisationDto = new OrganisationDto
            {
                Id = 4,
                Name = "Test Organisation",
                Description = "Test Organisation Description",
            }

        };

        CreateOrUpdateServiceCommand command = new(service);
        CreateOrUpdateServiceCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());
        var dbService = mockApplicationDbContext.ReferralServices.FirstOrDefault(x => x.Id == 4);

        //Assert
        result.Should().BeGreaterThan(0);
        result.Should().Be(service.Id);
        ArgumentNullException.ThrowIfNull(dbService);
        dbService.Name.Should().Be(service.Name);
        dbService.Description.Should().Be(service.Description);
    }

    [Fact]
    public async Task ThenUpdateService()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateOrUpdateServiceCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.ReferralServices.Add(new Data.Entities.ReferralService
        {
            Id = 4,
            Name = "Test Service",
            Description = "Test Service Description",
            Organisation = new Organisation
            {
                Id = 4,
                ReferralServiceId = 4,
                Name = "Test Organisation",
                Description = "Test Organisation Description",
            }
        });
        mockApplicationDbContext.SaveChanges();
        var service = new ReferralServiceDto
        {
            Id = 4,
            Name = "Test Service - Updated",
            Description = "Test Organisation Service - Updated",
            OrganisationDto = new OrganisationDto
            {
                Id = 4,
                ReferralServiceId = 4,
                Name = "Test Organisation - Updated",
                Description = "Test Organisation Description - Updated",
            }

        };
        CreateOrUpdateServiceCommand command = new(service);
        CreateOrUpdateServiceCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());
        var dbService = mockApplicationDbContext.ReferralServices
                        .Include(x => x.Organisation)
                        .FirstOrDefault(x => x.Id == 4);

        //Assert
        result.Should().BeGreaterThan(0);
        result.Should().Be(service.Id);
        ArgumentNullException.ThrowIfNull(dbService);
        dbService.Name.Should().Be(service.Name);
        dbService.Description.Should().Be(service.Description);
        dbService.Organisation.Name.Should().Be(service.OrganisationDto.Name);
        dbService.Organisation.Description.Should().Be(service.OrganisationDto.Description);

    }
}
