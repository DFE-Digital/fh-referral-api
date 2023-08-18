using Azure.Messaging.EventGrid.SystemEvents;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Models;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace FamilyHubs.Referral.FunctionalTests;

[Collection("Sequential")]
public class WhenUsingUserAccountsApiTests : BaseWhenUsingOpenReferralApiUnitTests
{
    [Fact]
    public async Task ThenSingleUserAccountsIsCreated()
    {
        if (!IsRunningLocally() || Client == null)
        {
            // Skip the test if not running locally
            Assert.True(true, "Test skipped because it is not running locally.");
            return;
        }

        var command = GetUserAccount();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Client.BaseAddress + "api/useraccount"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        long.TryParse(stringResult, out var result);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ThenTheUserAccountsAreCreated()
    {
        if (!IsRunningLocally() || Client == null)
        {
            // Skip the test if not running locally
            Assert.True(true, "Test skipped because it is not running locally.");
            return;
        }

        var command = new List<UserAccountDto> { GetUserAccount() };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Client.BaseAddress + "api/useraccounts"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        bool.TryParse(stringResult, out var result);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ThenSingleUserAccountIsCreatedThenUpdated()
    {
        if (!IsRunningLocally() || Client == null)
        {
            // Skip the test if not running locally
            Assert.True(true, "Test skipped because it is not running locally.");
            return;
        }

        var userAccount = GetUserAccount();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Client.BaseAddress + "api/useraccount"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(userAccount), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        long.TryParse(stringResult, out var result);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeGreaterThan(0);

        userAccount.EmailAddress = "MyChangedEmail@email.com";
        userAccount.PhoneNumber = "0161 222 2222";
        userAccount.Id = result;



        request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(Client.BaseAddress + $"api/useraccount/{userAccount.Id}"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(userAccount), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var updateresponse = await Client.SendAsync(request);

        updateresponse.EnsureSuccessStatusCode();

        stringResult = await updateresponse.Content.ReadAsStringAsync();
        bool.TryParse(stringResult, out bool updateresult);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        updateresult.Should().BeTrue();
    }

    [Fact]
    public async Task ThenTheUserAccountIsCreatedThenUpdated()
    {
        if (!IsRunningLocally() || Client == null)
        {
            // Skip the test if not running locally
            Assert.True(true, "Test skipped because it is not running locally.");
            return;
        }

        var userAccount = GetUserAccount();

        var command = new List<UserAccountDto> { userAccount };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Client.BaseAddress + "api/useraccounts"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        bool.TryParse(stringResult, out var result);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeTrue();

        userAccount.EmailAddress = "MyChangedEmail@email.com";
        userAccount.PhoneNumber = "0161 222 2222";

        command = new List<UserAccountDto> { userAccount };

        request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(Client.BaseAddress + "api/useraccounts"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var updateresponse = await Client.SendAsync(request);

        updateresponse.EnsureSuccessStatusCode();

        stringResult = await updateresponse.Content.ReadAsStringAsync();
        bool.TryParse(stringResult, out result);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ThenTheUserAccountIsCreatedThenRetrieved()
    {
        if (!IsRunningLocally() || Client == null)
        {
            // Skip the test if not running locally
            Assert.True(true, "Test skipped because it is not running locally.");
            return;
        }

        var userAccount = GetUserAccount();

        var command = new List<UserAccountDto> { userAccount };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Client.BaseAddress + "api/useraccounts"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        bool.TryParse(stringResult, out var result);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeTrue();

        long organisationId = 0;
        if (userAccount != null && userAccount.OrganisationUserAccounts != null && userAccount.OrganisationUserAccounts.Any())
        {
            organisationId = userAccount.OrganisationUserAccounts[0].Organisation.Id;
        }

        request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(Client.BaseAddress + $"api/useraccountsByOrganisationId/{organisationId}"),
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var getresponse = await Client.SendAsync(request);

        var retVal = await JsonSerializer.DeserializeAsync<PaginatedList<UserAccountDto>>(await getresponse.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        getresponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        ArgumentNullException.ThrowIfNull(retVal);
        ArgumentNullException.ThrowIfNull(userAccount);
        retVal.Items[0].Name.Should().Be(userAccount.Name);
        retVal.Items[0].PhoneNumber.Should().Be(userAccount.PhoneNumber);
        retVal.Items[0].EmailAddress.Should().Be(userAccount.EmailAddress);
#pragma warning disable CS8602        
        retVal.Items[0].OrganisationUserAccounts[0].Organisation.Should().BeEquivalentTo(userAccount.OrganisationUserAccounts[0].Organisation);
#pragma warning restore CS8602
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ThenSingleUserAccountsIsCreatedFromEvent(bool isValidationMessage)
    {
        if (!IsRunningLocally() || Client == null)
        {
            // Skip the test if not running locally
            Assert.True(true, "Test skipped because it is not running locally.");
            return;
        }

        // Check if it's a validation message

        HttpRequestMessage request = default!;
        if (isValidationMessage)
        {
            var command = new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    EventType = typeof(SubscriptionValidationEventData).AssemblyQualifiedName,
                    Subject = "Unit Test",
                    EventTime = DateTime.UtcNow,
                    Data = new
                    {
                        ValidationCode = "123456"
                    }
                }
            };

            request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Client.BaseAddress + "events"),
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
            };

        }
        else
        {
            // Set up the user account DTO for a regular event message
            UserAccountDto userAccountDto = new UserAccountDto
            {
                Id = 3,
                EmailAddress = "test@example.com",
                Name = "Test User",
                PhoneNumber = "123456789",
                Team = "Test Team"
            };

            userAccountDto.OrganisationUserAccounts = new List<UserAccountOrganisationDto>
            {
                new UserAccountOrganisationDto
                {
                    UserAccount = default!,
                    Organisation = new OrganisationDto
                    {
                        Id = 2,
                        Name = "Organisation",
                        Description = "Organisation Description",
                    }
                }
            };

            var command = new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    EventType ="UserAccountDto",
                    Subject = "Unit Test",
                    EventTime = DateTime.UtcNow,
                    Data = userAccountDto
                }
            };

            request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Client.BaseAddress + "events"),
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"),
            };

        }



        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{new JwtSecurityTokenHandler().WriteToken(_token)}");

        using var response = await Client.SendAsync(request);

        if (isValidationMessage)
        {
            // Assert that the response is a 200 OK status code
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
        else
        {
            // Assert that the response is a 201 Created status code or any other expected status code for regular event messages
            response.EnsureSuccessStatusCode();
        }
    }
}
