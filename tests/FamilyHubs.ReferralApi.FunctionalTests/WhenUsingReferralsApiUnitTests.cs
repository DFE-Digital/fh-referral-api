using FamilyHubs.ReferralApi.Data.Repository;
using FluentAssertions;
using System.Text;
using System.Text.Json;
using FamilyHubs.ServiceDirectory.Shared.Models;
using FamilyHubs.ReferralCommon.Shared.Dto;

namespace FamilyHubs.ReferralApi.FunctionalTests;

[Collection("Sequential")]
public class WhenUsingReferralsApiUnitTests : BaseWhenUsingOpenReferralApiUnitTests
{
    public WhenUsingReferralsApiUnitTests()
    {
        _client.BaseAddress = new Uri("https://localhost:7282/");
    }
#if UseJsonService
    const string JsonService = "{\"Id\":\"ba1cca90-b02a-4a0b-afa0-d8aed1083c0d\",\"Name\":\"Test County Council\",\"Description\":\"Test County Council\",\"Logo\":null,\"Uri\":\"https://www.test.gov.uk/\",\"Url\":\"https://www.test.gov.uk/\",\"Services\":[{\"Id\":\"c1b5dd80-7506-4424-9711-fe175fa13eb8\",\"Name\":\"Test Organisation for Children with Tracheostomies\",\"Description\":\"Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.\",\"Accreditations\":null,\"Assured_date\":null,\"Attending_access\":null,\"Attending_type\":null,\"Deliverable_type\":null,\"Status\":\"active\",\"Url\":\"www.testservice.com\",\"Email\":\"support@testservice.com\",\"Fees\":null,\"ServiceDelivery\":[{\"Id\":\"14db2aef-9292-4afc-be09-5f6f43765938\",\"ServiceDelivery\":2}],\"Eligibilities\":[{\"Id\":\"Test9109Children\",\"Eligibility\":\"\",\"Maximum_age\":0,\"Minimum_age\":13}],\"Contacts\":[{\"Id\":\"5eac5cb6-cc7e-444d-a29b-76ccb85be866\",\"Title\":\"Service\",\"Name\":\"\",\"Phones\":[{\"Id\":\"1568\",\"Number\":\"01827 65779\"}]}],\"Cost_options\":[],\"Languages\":[{\"Id\":\"442a06cd-aa14-4ea3-9f11-b45c1bc4861f\",\"Language\":\"English\"}],\"Service_areas\":[{\"Id\":\"68af19cd-bc81-4585-99a2-85a2b0d62a1d\",\"Service_area\":\"National\",\"Extent\":null,\"Uri\":\"http://statistics.data.gov.uk/id/statistical-geography/K02000001\"}],\"Service_at_locations\":[{\"Id\":\"Test1749\",\"Location\":{\"Id\":\"a878aadc-6097-4a0f-b3e1-77fd4511175d\",\"Name\":\"\",\"Description\":\"\",\"Latitude\":52.6312,\"Longitude\":-1.66526,\"Physical_addresses\":[{\"Id\":\"1076aaa8-f99d-4395-8e4f-c0dde8095085\",\"Address_1\":\"75 Sheepcote Lane\",\"City\":\", Stathe, Tamworth, Staffordshire, \",\"Postal_code\":\"B77 3JN\",\"Country\":\"England\",\"State_province\":null}]}}],\"Service_taxonomys\":[{\"Id\":\"Test9107\",\"Taxonomy\":{\"Id\":\"Test bccsource:Organisation\",\"Name\":\"Organisation\",\"Vocabulary\":\"Test BCC Data Sources\",\"Parent\":null}},{\"Id\":\"Test9108\",\"Taxonomy\":{\"Id\":\"Test bccprimaryservicetype:38\",\"Name\":\"Support\",\"Vocabulary\":\"Test BCC Primary Services\",\"Parent\":null}},{\"Id\":\"Test9109\",\"Taxonomy\":{\"Id\":\"Test bccagegroup:37\",\"Name\":\"Children\",\"Vocabulary\":\"Test BCC Age Groups\",\"Parent\":null}},{\"Id\":\"Test9110\",\"Taxonomy\":{\"Id\":\"Testbccusergroup:56\",\"Name\":\"Long Term Health Conditions\",\"Vocabulary\":\"Test BCC User Groups\",\"Parent\":null}}]}]}";
#endif

    [Fact]
    public async Task ThenReferralsByReferrerAreRetrieved()
    {
        var referrer = ReferralSeedData.SeedReferral().ElementAt(0).Referrer.EmailAddress;

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/referrals/{referrer}?pageNumber=1&pageSize=10"),
        };
#if UseAuthoriseHeader
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");
#endif
        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<PaginatedList<ReferralDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        ArgumentNullException.ThrowIfNull(retVal);
        retVal.Should().NotBeNull();
        retVal.Items.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ThenTheOpenReferralIsCreated()
    {
        var command = new ReferralDto
        { 
            Id = 2,
            ReferenceNumber = "1",
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
                EmailAddress = "Bob.Referrer@email.com", 
            },
            Status = new List<ReferralStatusDto>
            { 
                new ReferralStatusDto
                {
                    Status = "Inital-Referral"
                }
            },
            ReferralServiceDto = new ReferralServiceDto
            {
                Id = 2,
                Name = "Service",
                Description = "Service Description",
                ReferralOrganisationDto = new ReferralOrganisationDto
                { 
                    Id = 2,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }
            
        };

        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + "api/referrals"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
        };
#if UseAuthoriseHeader
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");
#endif
        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        long.TryParse(stringResult, out var result);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeGreaterThan(0);
    }

    

    [Fact]
    public async Task ThenReferralsByOrganisationIdAreRetrieved()
    {

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/organisationreferrals/1?pageNumber=1&pageSize=10"),
        };
#if UseAuthoriseHeader
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");
#endif
        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<PaginatedList<ReferralDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        ArgumentNullException.ThrowIfNull(retVal);
        retVal.Should().NotBeNull();
        retVal.Items.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ThenReferralByIdIsRetrieved()
    {

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/referral/1"),
        };
#if UseAuthoriseHeader
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");
#endif
        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<ReferralDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        ArgumentNullException.ThrowIfNull(retVal);
        retVal.Should().NotBeNull();
    }

    [Fact]
    public async Task ThenTheOpenReferralIsUpdated()
    {
        var command = new ReferralDto
        {
            Id = 1,
            ReferenceNumber = "1",
            ReasonForSupport = "Reason For Support",
            EngageWithFamily = "Engage With Family",
            RecipientDto = new RecipientDto
            {
                Id = 1,
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
                Id = 1,
                EmailAddress = "Bob.Referrer@email.com",
            },
            Status = new List<ReferralStatusDto>
            {
                new ReferralStatusDto
                {
                    Status = "Inital-Referral"
                }
            },
            ReferralServiceDto = new ReferralServiceDto
            {
                Id = 1,
                Name = "Service",
                Description = "Service Description",
                ReferralOrganisationDto = new ReferralOrganisationDto
                {
                    Id = 1,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }

        };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(_client.BaseAddress + "api/referrals/1"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
        };
#if UseAuthoriseHeader
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");
#endif
        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }


    [Fact]
    public async Task ThenTheOpenReferralStatusIsSet()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + "api/referralStatus/1/Accept Connection")
        };
#if UseAuthoriseHeader
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");
#endif
        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        stringResult.ToString().Should().Be("Accept Connection");
    }
    
    
    
    
}
