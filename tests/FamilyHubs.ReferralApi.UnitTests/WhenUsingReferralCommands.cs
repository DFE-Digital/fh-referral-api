using AutoMapper;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.ClientServices;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.SetReferralStatus;
using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FamilyHubs.Referral.Core.Queries.GetReferralStatus;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.ReferralService.Shared.Enums;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Identity.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.Referral.UnitTests;

public class WhenUsingReferralCommands : BaseCreateDbUnitTest
{
    private readonly Mock<IServiceDirectoryService> _serviceDirectoryService;
    public DateTimeOffset RequestTimestamp { get; set; }
    public FamilyHubsUser FamilyHubsUser { get; set; }
    public const long ExpectedAccountId = 123L;
    public const long ExpectedOrganisationId = 456L;

    public WhenUsingReferralCommands()
    {
        RequestTimestamp = new DateTimeOffset(new DateTime(2025, 1, 1, 12, 0, 0));

        FamilyHubsUser = new FamilyHubsUser
        {
            AccountId = ExpectedAccountId.ToString(),
            OrganisationId = ExpectedOrganisationId.ToString()
        };

        _serviceDirectoryService = new Mock<IServiceDirectoryService>();
        _serviceDirectoryService.Setup(x => x.GetOrganisationById(It.IsAny<long>())).ReturnsAsync(new ServiceDirectory.Shared.Dto.OrganisationDto
        {
            Id = 2,
            Name = "Organisation",
            Description = "Organisation Description",
            OrganisationType = ServiceDirectory.Shared.Enums.OrganisationType.VCFS,
            AdminAreaCode = "AdminAreaCode"
        });

        _serviceDirectoryService.Setup(x => x.GetServiceById(It.IsAny<long>())).ReturnsAsync(new ServiceDirectory.Shared.Dto.ServiceDto
        {
            Id = 2,
            Name = "Service",
            Description = "Service Description",
            ServiceType = ServiceDirectory.Shared.Enums.ServiceType.FamilyExperience
        });
    }

#if UseJsonService
        const string JsonService = "{\"Id\":\"ba1cca90-b02a-4a0b-afa0-d8aed1083c0d\",\"Name\":\"Test County Council\",\"Description\":\"Test County Council\",\"Logo\":null,\"Uri\":\"https://www.test.gov.uk/\",\"Url\":\"https://www.test.gov.uk/\",\"Services\":[{\"Id\":\"c1b5dd80-7506-4424-9711-fe175fa13eb8\",\"Name\":\"Test Organisation for Children with Tracheostomies\",\"Description\":\"Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.\",\"Accreditations\":null,\"Assured_date\":null,\"Attending_access\":null,\"Attending_type\":null,\"Deliverable_type\":null,\"Status\":\"active\",\"Url\":\"www.testservice.com\",\"Email\":\"support@testservice.com\",\"Fees\":null,\"ServiceDelivery\":[{\"Id\":\"14db2aef-9292-4afc-be09-5f6f43765938\",\"ServiceDelivery\":2}],\"Eligibilities\":[{\"Id\":\"Test9109Children\",\"Eligibility\":\"\",\"Maximum_age\":0,\"Minimum_age\":13}],\"Contacts\":[{\"Id\":\"5eac5cb6-cc7e-444d-a29b-76ccb85be866\",\"Title\":\"Service\",\"Name\":\"\",\"Phones\":[{\"Id\":\"1568\",\"Number\":\"01827 65779\"}]}],\"Cost_options\":[],\"Languages\":[{\"Id\":\"442a06cd-aa14-4ea3-9f11-b45c1bc4861f\",\"Language\":\"English\"}],\"Service_areas\":[{\"Id\":\"68af19cd-bc81-4585-99a2-85a2b0d62a1d\",\"Service_area\":\"National\",\"Extent\":null,\"Uri\":\"http://statistics.data.gov.uk/id/statistical-geography/K02000001\"}],\"Service_at_locations\":[{\"Id\":\"Test1749\",\"Location\":{\"Id\":\"a878aadc-6097-4a0f-b3e1-77fd4511175d\",\"Name\":\"\",\"Description\":\"\",\"Latitude\":52.6312,\"Longitude\":-1.66526,\"Physical_addresses\":[{\"Id\":\"1076aaa8-f99d-4395-8e4f-c0dde8095085\",\"Address_1\":\"75 Sheepcote Lane\",\"City\":\", Stathe, Tamworth, Staffordshire, \",\"Postal_code\":\"B77 3JN\",\"Country\":\"England\",\"State_province\":null}]}}],\"Service_taxonomys\":[{\"Id\":\"Test9107\",\"Taxonomy\":{\"Id\":\"Test bccsource:Organisation\",\"Name\":\"Organisation\",\"Vocabulary\":\"Test BCC Data Sources\",\"Parent\":null}},{\"Id\":\"Test9108\",\"Taxonomy\":{\"Id\":\"Test bccprimaryservicetype:38\",\"Name\":\"Support\",\"Vocabulary\":\"Test BCC Primary Services\",\"Parent\":null}},{\"Id\":\"Test9109\",\"Taxonomy\":{\"Id\":\"Test bccagegroup:37\",\"Name\":\"Children\",\"Vocabulary\":\"Test BCC Age Groups\",\"Parent\":null}},{\"Id\":\"Test9110\",\"Taxonomy\":{\"Id\":\"Testbccusergroup:56\",\"Name\":\"Long Term Health Conditions\",\"Vocabulary\":\"Test BCC User Groups\",\"Parent\":null}}]}]}";
#endif
    [Fact]
    public async Task ThenCreateReferral()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        await mockApplicationDbContext.SaveChangesAsync();

        var testReferral = GetReferralDto();
        testReferral.Status.Id = 0;
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Id.Should().BeGreaterThan(0);
        result.Id.Should().Be(testReferral.Id);
        result.ServiceName.Should().Be(testReferral.ReferralServiceDto.Name);
    }

    [Fact]
    public async Task ThenCreateReferralWithOrganisationNullReturnFromServiceDirectoryService()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();

        ServiceDirectory.Shared.Dto.OrganisationDto? organisation = null;
        _serviceDirectoryService.Setup(x => x.GetOrganisationById(It.IsAny<long>())).ReturnsAsync(organisation);

        var testReferral = GetReferralDto();
        testReferral.Status.Id = 0;
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Failed to return Organisation from service directory for Id = 0");
    }

    [Fact]
    public async Task ThenCreateReferralWithServiceNullReturnFromServiceDirectoryService()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();

        ServiceDirectory.Shared.Dto.ServiceDto? service = null;
        _serviceDirectoryService.Setup(x => x.GetServiceById(It.IsAny<long>())).ReturnsAsync(service);

        var testReferral = GetReferralDto();
        testReferral.Status.Id = 0;
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Failed to return Service from service directory for Id = 2");
    }

    [Fact]
    public async Task ThenCreateReferralWithExitingReferrer()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Referrals.Add(ReferralSeedData.SeedReferral().ElementAt(0));
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        testReferral.ReferralUserAccountDto = mapper.Map<UserAccountDto>(ReferralSeedData.SeedReferral().ElementAt(0).UserAccount);
        testReferral.ReferralUserAccountDto.Id = mockApplicationDbContext.UserAccounts.First().Id;
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Id.Should().BeGreaterThan(0);
        result.Id.Should().Be(testReferral.Id);
        result.ServiceName.Should().Be(testReferral.ReferralServiceDto.Name);
    }

    [Fact]
    public async Task ThenCreateReferralWithExitingRecipientEmail()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();

        var referral = ReferralSeedData.SeedReferral().ElementAt(0);
        var status = mockApplicationDbContext.Statuses.SingleOrDefault(x => x.Name == referral.Status.Name);
        if (status != null)
        {
            referral.Status = status;
        }
        mockApplicationDbContext.Referrals.Add(referral);
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        testReferral.RecipientDto = new RecipientDto
        {
            Id = mockApplicationDbContext.Recipients.First().Id,
            Name = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Name,
            Email = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Email,
        };
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Id.Should().BeGreaterThan(0);
        result.Id.Should().Be(testReferral.Id);
        result.ServiceName.Should().Be(testReferral.ReferralServiceDto.Name);
    }

    [Fact]
    public async Task ThenCreateReferralWithExitingRecipientTelephone()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();

        var referral = ReferralSeedData.SeedReferral().ElementAt(0);
        var status = mockApplicationDbContext.Statuses.SingleOrDefault(x => x.Name == referral.Status.Name);
        if (status != null)
        {
            referral.Status = status;
        }
        mockApplicationDbContext.Referrals.Add(referral);
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        testReferral.RecipientDto = new RecipientDto
        {
            Id = mockApplicationDbContext.Recipients.First().Id,
            Name = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Name,
            Telephone = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Telephone,
        };
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Id.Should().BeGreaterThan(0);
        result.Id.Should().Be(testReferral.Id);
        result.ServiceName.Should().Be(testReferral.ReferralServiceDto.Name);
    }

    [Fact]
    public async Task ThenCreateReferralWithExitingRecipientTextphone()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();

        var referral = ReferralSeedData.SeedReferral().ElementAt(0);
        var status = mockApplicationDbContext.Statuses.SingleOrDefault(x => x.Name == referral.Status.Name);
        if (status != null)
        {
            referral.Status = status;
        }
        mockApplicationDbContext.Referrals.Add(referral);
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        testReferral.RecipientDto = new RecipientDto
        {
            Id = mockApplicationDbContext.Recipients.First().Id,
            Name = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Name,
            TextPhone = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.TextPhone,
        };
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Id.Should().BeGreaterThan(0);
        result.Id.Should().Be(testReferral.Id);
        result.ServiceName.Should().Be(testReferral.ReferralServiceDto.Name);
    }

    [Fact]
    public async Task ThenCreateReferralWithExitingRecipientNameAndPostCode()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();

        var referral = ReferralSeedData.SeedReferral().ElementAt(0);
        var status = mockApplicationDbContext.Statuses.SingleOrDefault(x => x.Name == referral.Status.Name);
        if (status != null)
        {
            referral.Status = status;
        }
        mockApplicationDbContext.Referrals.Add(referral);
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        testReferral.RecipientDto = new RecipientDto
        {
            Id = mockApplicationDbContext.Recipients.First().Id,
            Name = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Name,
            PostCode = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.PostCode,
        };
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Id.Should().BeGreaterThan(0);
        result.Id.Should().Be(testReferral.Id);
        result.ServiceName.Should().Be(testReferral.ReferralServiceDto.Name);
    }

    [Fact]
    public async Task ThenCreateReferralWithExitingServiceAndUpdateSubProporties()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();

        var referral = ReferralSeedData.SeedReferral().ElementAt(0);
        var status = mockApplicationDbContext.Statuses.SingleOrDefault(x => x.Name == referral.Status.Name);
        if (status != null)
        {
            referral.Status = status;
        }
        mockApplicationDbContext.Referrals.Add(referral);
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        testReferral.ReferralServiceDto = mapper.Map<ReferralServiceDto>(ReferralSeedData.SeedReferral().ElementAt(0).ReferralService);

        testReferral.ReasonForSupport = "New Reason For Support";
        testReferral.EngageWithFamily = "New Engage With Family";
        testReferral.RecipientDto.Telephone = "078123459";
        testReferral.RecipientDto.TextPhone = "078123459";
        testReferral.RecipientDto.AddressLine1 = "Address Line 1A";
        testReferral.RecipientDto.AddressLine2 = "Address Line 2A";
        testReferral.RecipientDto.TownOrCity = "Town or CityA";
        testReferral.RecipientDto.County = "CountyA";
        testReferral.ReferralUserAccountDto.PhoneNumber = "1234567899";
        testReferral.ReferralServiceDto.Id = 0;
        testReferral.ReferralServiceDto.Name = "Service A";
        testReferral.ReferralServiceDto.Description = "Service Description A";
        testReferral.ReferralServiceDto.OrganisationDto.Id = 0;
        testReferral.ReferralServiceDto.OrganisationDto.Name = "Organisation A";
        testReferral.ReferralServiceDto.OrganisationDto.Description = "Organisation Description A";

        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Id.Should().BeGreaterThan(0);
        result.Id.Should().Be(testReferral.Id);
        result.ServiceName.Should().Be(testReferral.ReferralServiceDto.Name);

        GetReferralByIdCommand getcommand = new(result.Id);
        GetReferralByIdCommandHandler gethandler = new(mockApplicationDbContext, mapper);


        //Check and Assert
        var getresult = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());
        testReferral.ReferralServiceDto.Id = 0;
        testReferral.ReferralServiceDto.OrganisationDto.ReferralServiceId = 0;
        testReferral.Status.SecondrySortOrder = 1;
        getresult.Should().BeEquivalentTo(testReferral, options => options.Excluding(x => x.Created).Excluding(x => x.LastModified).Excluding(x => x.ReferralUserAccountDto.UserAccountRoles));


    }

    [Fact]
    public async Task ThenUpdateReferral()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand createCommand = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler createHandler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);
        var setResult = await createHandler.Handle(createCommand, new System.Threading.CancellationToken());
        testReferral.RecipientDto.Name = testReferral.RecipientDto.Name + " Test";
        testReferral.RecipientDto.Email = testReferral.RecipientDto.Email + " Test";
        testReferral.RecipientDto.Telephone = testReferral.RecipientDto.Telephone + " Test";
        testReferral.RecipientDto.TextPhone = testReferral.RecipientDto.TextPhone + " Test";
        testReferral.ReasonForSupport = testReferral.ReasonForSupport + " Test";
        UpdateReferralCommand command = new(setResult.Id, testReferral);
        UpdateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        setResult.Id.Should().BeGreaterThan(0);
        setResult.Id.Should().Be(testReferral.Id);
        setResult.ServiceName.Should().Be(testReferral.ReferralServiceDto.Name);
        result.Should().BeGreaterThan(0);
        result.Should().Be(testReferral.Id);
    }

    [Theory]
    [InlineData(null, null, 1)]
    [InlineData(ReferralOrderBy.NotSet, true, 1)]
    [InlineData(ReferralOrderBy.DateSent, true, 1)]
    [InlineData(ReferralOrderBy.DateSent, false, 2)]
    [InlineData(ReferralOrderBy.DateUpdated, true, 1)]
    [InlineData(ReferralOrderBy.DateUpdated, false, 2)]
    [InlineData(ReferralOrderBy.Status, true, 2)] //--
    [InlineData(ReferralOrderBy.Status, false, 2)]
    [InlineData(ReferralOrderBy.RecipientName, true, 1)]
    [InlineData(ReferralOrderBy.RecipientName, false, 2)]
    [InlineData(ReferralOrderBy.Team, true, 1)]
    [InlineData(ReferralOrderBy.Team, false, 1)]
    [InlineData(ReferralOrderBy.ServiceName, true, 1)]
    [InlineData(ReferralOrderBy.ServiceName, false, 1)]
    public async Task ThenGetReferralsByReferrer(ReferralOrderBy? referralOrderBy, bool? isAssending, int firstId)
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var mockApplicationDbContext = GetApplicationDbContext();
        await CreateReferrals(mockApplicationDbContext);


        GetReferralsByReferrerCommand getcommand = new("Joe.Professional@email.com", referralOrderBy, isAssending, null, 1, 10);
        GetReferralsByReferrerCommandHandler gethandler = new(mockApplicationDbContext, mapper);


        //Act
        var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(2);
        result.Items[0].Created.Should().NotBeNull();
        result.Items[0].Id.Should().Be(firstId);
    }

    //
    [Theory]
    [InlineData(null, null, 1)]
    [InlineData(ReferralOrderBy.NotSet, true, 1)]
    [InlineData(ReferralOrderBy.DateSent, true, 1)]
    [InlineData(ReferralOrderBy.DateSent, false, 2)]
    [InlineData(ReferralOrderBy.DateUpdated, true, 1)]
    [InlineData(ReferralOrderBy.DateUpdated, false, 2)]
    [InlineData(ReferralOrderBy.Status, true, 2)]
    [InlineData(ReferralOrderBy.Status, false, 2)]
    [InlineData(ReferralOrderBy.RecipientName, true, 1)]
    [InlineData(ReferralOrderBy.RecipientName, false, 2)]
    [InlineData(ReferralOrderBy.Team, true, 1)]
    [InlineData(ReferralOrderBy.Team, false, 1)]
    [InlineData(ReferralOrderBy.ServiceName, true, 1)]
    [InlineData(ReferralOrderBy.ServiceName, false, 1)]
    public async Task ThenGetReferralsByReferrerId(ReferralOrderBy? referralOrderBy, bool? isAssending, int firstId)
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var mockApplicationDbContext = GetApplicationDbContext();
        await CreateReferrals(mockApplicationDbContext);


        GetReferralsByReferrerByReferrerIdCommand getcommand = new(5, referralOrderBy, isAssending, false, 1, 10);
        GetReferralsByReferrerByReferrerIdCommandHandler gethandler = new(mockApplicationDbContext, mapper);


        //Act
        var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(2);
        result.Items[0].Created.Should().NotBeNull();
        result.Items[0].Id.Should().Be(firstId);
    }
    //

    [Theory]
    [InlineData(ReferralOrderBy.DateSent, true, 1)]
    [InlineData(ReferralOrderBy.DateSent, false, 2)]
    [InlineData(ReferralOrderBy.DateUpdated, true, 1)]
    [InlineData(ReferralOrderBy.DateUpdated, false, 2)]
    [InlineData(ReferralOrderBy.Status, true, 1)]
    [InlineData(ReferralOrderBy.Status, false, 2)]
    [InlineData(ReferralOrderBy.RecipientName, true, 1)]
    [InlineData(ReferralOrderBy.RecipientName, false, 2)]
    [InlineData(ReferralOrderBy.Team, true, 1)]
    [InlineData(ReferralOrderBy.Team, false, 1)]
    [InlineData(ReferralOrderBy.ServiceName, true, 1)]
    [InlineData(ReferralOrderBy.ServiceName, false, 1)]
    public async Task ThenGetReferralsByOrganisationId(ReferralOrderBy referralOrderBy, bool isAssending, int firstId)
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var mockApplicationDbContext = GetApplicationDbContext();
        await CreateReferrals(mockApplicationDbContext);
        GetReferralsByOrganisationIdCommand getcommand = new(1, referralOrderBy, isAssending, null, 1, 10);
        GetReferralsByOrganisationIdCommandHandler gethandler = new(mockApplicationDbContext, mapper);


        //Act
        var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(2);
        result.Items[0].Created.Should().NotBeNull();
        result.Items[0].Id.Should().Be(firstId);
    }



    [Fact]
    public async Task ThenGetReferralsById()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        var testReferral = GetReferralDto();
        testReferral.ReasonForDecliningSupport = "Reason For Declining Support";
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);
        var response = await handler.Handle(command, new System.Threading.CancellationToken());

        GetReferralByIdCommand getcommand = new(response.Id);
        GetReferralByIdCommandHandler gethandler = new(mockApplicationDbContext, mapper);


        //Act
        var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Created.Should().NotBeNull();
        result.ReasonForDecliningSupport.Should().Be("Reason For Declining Support");

    }

    [Fact]
    public async Task ThenGetReferralStatusList()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var mockApplicationDbContext = GetApplicationDbContext();
        IReadOnlyCollection<Status> statuses = ReferralSeedData.SeedStatuses();
        mockApplicationDbContext.Statuses.AddRange(statuses);
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        await mockApplicationDbContext.SaveChangesAsync();
        GetReferralStatusesCommand command = new();
        GetReferralStatusesCommandHandler handler = new(mockApplicationDbContext, mapper);


        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(statuses.Count);
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData(null, 1L, null, null)]
    [InlineData(null, null, 1L, null)]
    [InlineData(null, null, null, 2L)]
    [InlineData(2L, 1L, 1L, 2L)]
    public async Task ThenGetReferralsByCompositeKey(long? serviceId, long? statusId, long? recipientId, long? referralId)
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        var testReferral = GetReferralDto();
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand command = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);
        await handler.Handle(command, new System.Threading.CancellationToken());

        GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand getcommand = new(serviceId, statusId, recipientId, referralId, ReferralOrderBy.RecipientName, true, null, 1, 10);
        GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommandHandler gethandler = new(mockApplicationDbContext, mapper);


        //Act
        var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
    }

    public static ReferralDto GetReferralDto()
    {
        return new ReferralDto
        {
            Id = 2,
            ReasonForSupport = "Reason For Support",
            EngageWithFamily = "Engage With Family",
            RecipientDto = new RecipientDto
            {
                Id = 2,
                Name = "Joe Blogs",
                Email = "JoeBlog@email.com",
                Telephone = "078123456",
                TextPhone = "078123456",
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                TownOrCity = "Town or City",
                County = "County",
                PostCode = "B30 2TV"
            },
            ReferralUserAccountDto = new UserAccountDto
            {
                Id = 2,
                EmailAddress = "Bob.Referrer@email.com",
                Name = "Bob Referrer",
                PhoneNumber = "1234567890",
                Team = "Team",
                UserAccountRoles = new List<UserAccountRoleDto>()
                {
                    new UserAccountRoleDto
                    {
                        UserAccount = new UserAccountDto
                        {
                            EmailAddress = "Bob.Referrer@email.com",
                        },
                        Role = new RoleDto
                        {
                            Name = "VcsProfessional"
                        }
                    }
                },
                ServiceUserAccounts = new List<UserAccountServiceDto>(),
                OrganisationUserAccounts = new List<UserAccountOrganisationDto>(),
            },
            Status = new ReferralStatusDto
            {
                Id = 1,
                Name = "New",
                SortOrder = 0
            },
            ReferralServiceDto = new ReferralServiceDto
            {
                Id = 2,
                Name = "Service",
                Description = "Service Description",
                Url = "www.service.com",
                OrganisationDto = new OrganisationDto
                {
                    Id = 2,
                    ReferralServiceId = 2,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }
        };
    }

    [Theory]
    [InlineData(RoleTypes.DfeAdmin)]
    [InlineData(RoleTypes.VcsProfessional)]
    [InlineData(RoleTypes.VcsDualRole)]
    public async Task ThenUpdateStatusOfReferral(string role)
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand createCommand = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler createHandler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);
        var setupresult = await createHandler.Handle(createCommand, new System.Threading.CancellationToken());
        SetReferralStatusCommand command = new(role, testReferral.ReferralServiceDto.OrganisationDto.Id, testReferral.Id, "Declined", "Unable to help");
        SetReferralStatusCommandHandler handler = new(mockApplicationDbContext, new Mock<ILogger<SetReferralStatusCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        setupresult.Id.Should().BeGreaterThan(0);
        setupresult.Id.Should().Be(testReferral.Id);
        result.Should().NotBeNull();
        result.Should().Be("Declined");
    }

    [Fact]
    public async Task ThenUpdateStatusOfReferralReturnsForbidden()
    {
        //Arange
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());
        mockApplicationDbContext.Roles.AddRange(ReferralSeedData.SeedRoles());
        mockApplicationDbContext.SaveChanges();
        var testReferral = GetReferralDto();
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));
        CreateReferralCommand createCommand = new(createReferral, FamilyHubsUser);
        CreateReferralCommandHandler createHandler = new(mockApplicationDbContext, mapper, _serviceDirectoryService.Object, logger.Object);
        var setupresult = await createHandler.Handle(createCommand, new System.Threading.CancellationToken());
        SetReferralStatusCommand command = new(RoleTypes.LaProfessional, -1, testReferral.Id, "Declined", "Unable to help");
        SetReferralStatusCommandHandler handler = new(mockApplicationDbContext, new Mock<ILogger<SetReferralStatusCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        setupresult.Id.Should().BeGreaterThan(0);
        setupresult.Id.Should().Be(testReferral.Id);
        result.Should().NotBeNull();
        result.Should().Be(SetReferralStatusCommandHandler.Forbidden);
    }
}