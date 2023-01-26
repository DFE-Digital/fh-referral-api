using AutoMapper;
using FamilyHubs.ReferralApi.Api.Commands.CreateReferral;
using FamilyHubs.ReferralApi.Api.Commands.SetReferralStatus;
using FamilyHubs.ReferralApi.Api.Commands.UpdateReferral;
using FamilyHubs.ReferralApi.Api.Queries.GetReferrals;
using FamilyHubs.ReferralApi.Core;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.Referrals;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using static System.Net.Mime.MediaTypeNames;

namespace FamilyHubs.ReferralApi.UnitTests
{
    public class WhenUsingReferralCommands : BaseCreateDbUnitTest
    {
        const string JsonService = "{\"Id\":\"ba1cca90-b02a-4a0b-afa0-d8aed1083c0d\",\"Name\":\"Test County Council\",\"Description\":\"Test County Council\",\"Logo\":null,\"Uri\":\"https://www.test.gov.uk/\",\"Url\":\"https://www.test.gov.uk/\",\"Services\":[{\"Id\":\"c1b5dd80-7506-4424-9711-fe175fa13eb8\",\"Name\":\"Test Organisation for Children with Tracheostomies\",\"Description\":\"Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.\",\"Accreditations\":null,\"Assured_date\":null,\"Attending_access\":null,\"Attending_type\":null,\"Deliverable_type\":null,\"Status\":\"active\",\"Url\":\"www.testservice.com\",\"Email\":\"support@testservice.com\",\"Fees\":null,\"ServiceDelivery\":[{\"Id\":\"14db2aef-9292-4afc-be09-5f6f43765938\",\"ServiceDelivery\":2}],\"Eligibilities\":[{\"Id\":\"Test9109Children\",\"Eligibility\":\"\",\"Maximum_age\":0,\"Minimum_age\":13}],\"Contacts\":[{\"Id\":\"5eac5cb6-cc7e-444d-a29b-76ccb85be866\",\"Title\":\"Service\",\"Name\":\"\",\"Phones\":[{\"Id\":\"1568\",\"Number\":\"01827 65779\"}]}],\"Cost_options\":[],\"Languages\":[{\"Id\":\"442a06cd-aa14-4ea3-9f11-b45c1bc4861f\",\"Language\":\"English\"}],\"Service_areas\":[{\"Id\":\"68af19cd-bc81-4585-99a2-85a2b0d62a1d\",\"Service_area\":\"National\",\"Extent\":null,\"Uri\":\"http://statistics.data.gov.uk/id/statistical-geography/K02000001\"}],\"Service_at_locations\":[{\"Id\":\"Test1749\",\"Location\":{\"Id\":\"a878aadc-6097-4a0f-b3e1-77fd4511175d\",\"Name\":\"\",\"Description\":\"\",\"Latitude\":52.6312,\"Longitude\":-1.66526,\"Physical_addresses\":[{\"Id\":\"1076aaa8-f99d-4395-8e4f-c0dde8095085\",\"Address_1\":\"75 Sheepcote Lane\",\"City\":\", Stathe, Tamworth, Staffordshire, \",\"Postal_code\":\"B77 3JN\",\"Country\":\"England\",\"State_province\":null}]}}],\"Service_taxonomys\":[{\"Id\":\"Test9107\",\"Taxonomy\":{\"Id\":\"Test bccsource:Organisation\",\"Name\":\"Organisation\",\"Vocabulary\":\"Test BCC Data Sources\",\"Parent\":null}},{\"Id\":\"Test9108\",\"Taxonomy\":{\"Id\":\"Test bccprimaryservicetype:38\",\"Name\":\"Support\",\"Vocabulary\":\"Test BCC Primary Services\",\"Parent\":null}},{\"Id\":\"Test9109\",\"Taxonomy\":{\"Id\":\"Test bccagegroup:37\",\"Name\":\"Children\",\"Vocabulary\":\"Test BCC Age Groups\",\"Parent\":null}},{\"Id\":\"Test9110\",\"Taxonomy\":{\"Id\":\"Testbccusergroup:56\",\"Name\":\"Long Term Health Conditions\",\"Vocabulary\":\"Test BCC User Groups\",\"Parent\":null}}]}]}";

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
            result.Should().NotBeNull();
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
            testReferral.FullName = testReferral.FullName + " Test";
            testReferral.Email = testReferral.Email + " Test";
            testReferral.Phone = testReferral.Phone + " Test";
            testReferral.Text = testReferral.Text + " Test";
            testReferral.ReasonForSupport = testReferral.ReasonForSupport + " Test";
            UpdateReferralCommand command = new(testReferral.Id, testReferral);
            UpdateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            setResult.Should().NotBeNull();
            setResult.Should().Be(testReferral.Id);
            result.Should().NotBeNull();
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
            var id = await handler.Handle(command, new System.Threading.CancellationToken());

            GetReferralsByReferrerCommand getcommand = new("Bob Referrer",1,10);
            GetReferralsByReferrerCommandHandler gethandler = new(mockApplicationDbContext);
            

            //Act
            var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Items[0].Should().BeEquivalentTo(testReferral);
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
            var id = await handler.Handle(command, new System.Threading.CancellationToken());

            GetReferralsByOrganisationIdCommand getcommand = new("ba1cca90-b02a-4a0b-afa0-d8aed1083c0d", 1, 10);
            GetReferralsByOrganisationIdCommandHandler gethandler = new(mockApplicationDbContext);


            //Act
            var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Items[0].Should().BeEquivalentTo(testReferral);
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
            CreateReferralCommand command = new(testReferral);
            CreateReferralCommandHandler handler = new(mockApplicationDbContext, mapper, logger.Object);
            var id = await handler.Handle(command, new System.Threading.CancellationToken());

            GetReferralByIdCommand getcommand = new("212fecf5-0db2-4e05-b28c-8cacaebba840");
            GetReferralByIdCommandHandler gethandler = new(mockApplicationDbContext);


            //Act
            var result = await gethandler.Handle(getcommand, new System.Threading.CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(testReferral);
        }

        public static ReferralDto GetReferralDto()
        {
            return new ReferralDto(
                "212fecf5-0db2-4e05-b28c-8cacaebba840",
                "ba1cca90-b02a-4a0b-afa0-d8aed1083c0d",
                "c1b5dd80-7506-4424-9711-fe175fa13eb8",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "Bob Referrer",
                "Mr Robert Brown",
                "No",
                "Robert.Brown@yahoo.co.uk",
                "0131 222 3333",
                "",
                "Requires help with child",
                null,
                new List<ReferralStatusDto> { new ReferralStatusDto("aa0be0f8-c218-401d-9237-aa75b6e38f01", "Inital-Referral") }
                );
        }

        [Fact]
        public async Task ThenUpdateStatusOfReferral()
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
            var setupresult = await createHandler.Handle(createCommand, new System.Threading.CancellationToken());
            SetReferralStatusCommand command = new(testReferral.Id, "Accept Connection");
            CreateReferralStatusCommandHandler handler = new(mockApplicationDbContext, new Mock<ILogger<CreateReferralStatusCommandHandler>>().Object);

            //Act
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            //Assert
            setupresult.Should().NotBeNull();
            setupresult.Should().Be(testReferral.Id);
            result.Should().NotBeNull();
            result.Should().Be("Accept Connection");

        }
    }
}