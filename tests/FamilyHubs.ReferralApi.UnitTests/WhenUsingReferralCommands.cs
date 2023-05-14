using AutoMapper;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace FamilyHubs.Referral.UnitTests
{
    public class WhenUsingReferralCommands : BaseCreateDbUnitTest
    {
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
            var testReferral = GetReferralDto();
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().Be(testReferral.Id);
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
            testReferral.ReferrerDto = mapper.Map<ReferrerDto>(ReferralSeedData.SeedReferral().ElementAt(0).Referrer);
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().Be(testReferral.Id);
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
            mockApplicationDbContext.Referrals.Add(ReferralSeedData.SeedReferral().ElementAt(0));
            mockApplicationDbContext.SaveChanges();
            var testReferral = GetReferralDto();
            testReferral.RecipientDto = new RecipientDto
            { 
                Name = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Name,
                Email = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Email,
            };
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().Be(testReferral.Id);
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
            mockApplicationDbContext.Referrals.Add(ReferralSeedData.SeedReferral().ElementAt(0));
            mockApplicationDbContext.SaveChanges();
            var testReferral = GetReferralDto();
            testReferral.RecipientDto = new RecipientDto
            {
                Name = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Name,
                Telephone = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Telephone,
            };
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().Be(testReferral.Id);
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
            mockApplicationDbContext.Referrals.Add(ReferralSeedData.SeedReferral().ElementAt(0));
            mockApplicationDbContext.SaveChanges();
            var testReferral = GetReferralDto();
            testReferral.RecipientDto = new RecipientDto
            {
                Name = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Name,
                TextPhone = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.TextPhone,
            };
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().Be(testReferral.Id);
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
            mockApplicationDbContext.Referrals.Add(ReferralSeedData.SeedReferral().ElementAt(0));
            mockApplicationDbContext.SaveChanges();
            var testReferral = GetReferralDto();
            testReferral.RecipientDto = new RecipientDto
            {
                Name = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.Name,
                PostCode = ReferralSeedData.SeedReferral().ElementAt(0).Recipient.PostCode,
            };
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().Be(testReferral.Id);
        }

        [Fact]
        public async Task ThenCreateReferralWithExitingService()
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
            testReferral.ReferralServiceDto = mapper.Map<ReferralServiceDto>(ReferralSeedData.SeedReferral().ElementAt(0).Service);

            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().Be(testReferral.Id);
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
            var testReferral = GetReferralDto();
            CreateReferralCommand createCommand = new(testReferral);
            CreateReferralCommandHandler createHandler = new(mockApplicationDbContext, mapper, logger.Object);
            var setResult = await createHandler.Handle(createCommand, new System.Threading.CancellationToken());
            testReferral.RecipientDto.Name = testReferral.RecipientDto.Name + " Test";
            testReferral.RecipientDto.Email = testReferral.RecipientDto.Email + " Test";
            testReferral.RecipientDto.Telephone = testReferral.RecipientDto.Telephone + " Test";
            testReferral.RecipientDto.TextPhone = testReferral.RecipientDto.TextPhone + " Test";
            testReferral.ReasonForSupport = testReferral.ReasonForSupport + " Test";
            UpdateReferralCommand command = new(setResult, testReferral);
            UpdateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            setResult.Should().BeGreaterThan(0);
            setResult.Should().Be(testReferral.Id);
            result.Should().BeGreaterThan(0);
            result.Should().Be(testReferral.Id);
        }

        [Fact]
        public async Task ThenGetReferralsByReferrer()
        {
            //Arange
            var myProfile = new AutoMappingProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
            var mockApplicationDbContext = GetApplicationDbContext();
            var testReferral = GetReferralDto();
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);
            await handler.Handle(command, new System.Threading.CancellationToken());

            GetReferralsByReferrerCommand getcommand = new("Bob.Users@email.com", 1,10, null);
            GetReferralsByReferrerCommandHandler gethandler = new(mockApplicationDbContext, mapper);
            

            //Act
            var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Items.Count.Should().Be(1);
            result.Items[0].Created.Should().NotBeNull();
        }

        [Theory]
        [InlineData("0002")]
        [InlineData("Joe Blogs")]
        [InlineData("0002 Joe Blogs")]
        public async Task ThenGetReferralsByReferrerWithSearchText(string searchText)
        {
            //Arange
            var myProfile = new AutoMappingProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
            var mockApplicationDbContext = GetApplicationDbContext();
            var testReferral = GetReferralDto();
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);
            await handler.Handle(command, new System.Threading.CancellationToken());

            GetReferralsByReferrerCommand getcommand = new("Bob.Users@email.com", 1, 10, searchText);
            GetReferralsByReferrerCommandHandler gethandler = new(mockApplicationDbContext, mapper);


            //Act
            var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Items.Count.Should().Be(1);
        }

        [Fact]
        public async Task ThenGetReferralsByOrganisationId()
        {
            //Arange
            var myProfile = new AutoMappingProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
            var mockApplicationDbContext = GetApplicationDbContext();
            var testReferral = GetReferralDto();
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);
            await handler.Handle(command, new System.Threading.CancellationToken());

            GetReferralsByOrganisationIdCommand getcommand = new(2, 1, 10, null);
            GetReferralsByOrganisationIdCommandHandler gethandler = new(mockApplicationDbContext, mapper);


            //Act
            var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Items.Count.Should().Be(1);
            result.Items[0].Created.Should().NotBeNull();
        }

        [Theory]
        [InlineData("0002")]
        [InlineData("Joe Blogs")]
        [InlineData("0002 Joe Blogs")]
        public async Task ThenGetReferralsByOrganisationIdWithSearchText(string searchText)
        {
            //Arange
            var myProfile = new AutoMappingProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            var logger = new Mock<ILogger<CreateReferralCommandHandler>>();
            var mockApplicationDbContext = GetApplicationDbContext();
            var testReferral = GetReferralDto();
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);
            await handler.Handle(command, new System.Threading.CancellationToken());

            GetReferralsByOrganisationIdCommand getcommand = new(2, 1, 10, searchText);
            GetReferralsByOrganisationIdCommandHandler gethandler = new(mockApplicationDbContext, mapper);


            //Act
            var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Items.Count.Should().Be(1);
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
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);
            var id = await handler.Handle(command, new System.Threading.CancellationToken());

            GetReferralByIdCommand getcommand = new(id);
            GetReferralByIdCommandHandler gethandler = new(mockApplicationDbContext, mapper);


            //Act
            var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Created.Should().NotBeNull();
            result.Should().BeEquivalentTo(testReferral, options => options.Excluding(x => x.Created).Excluding(x => x.LastModified));
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
                    Country = "Country",
                    PostCode = "B30 2TV"
                },
                ReferrerDto = new ReferrerDto
                {
                    Id = 2,
                    EmailAddress = "Bob.Users@email.com",
                    Name = "Bob Users",
                    PhoneNumber = "1234567890",
                    Role = "Role",
                    Team = "Teams"
                },
                Status = new List<ReferralStatusDto>
                {
                    new ReferralStatusDto
                    {
                        Id = 2,
                        Status = "New"
                    }
                },
                ReferralServiceDto = new ReferralServiceDto
                {
                    Id = 2,
                    Name = "Services",
                    Description = "Services Description",
                    ReferralOrganisationDto = new ReferralOrganisationDto
                    {
                        Id = 2,
                        Name = "Organisation",
                        Description = "Organisation Description",
                    }
                }

            };
        }
    }
}