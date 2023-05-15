

using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.Referral.Integration.Tests;

public static class TestDataProvider
{

    public static ReferralDto GetReferralDto()
    {
        return new ReferralDto
        {
            Id = 2,
            ReasonForSupport = "Reason For Support",
            EngageWithFamily = "Engage With Family",
            Created = DateTime.UtcNow,
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
            UserDto = new UserDto
            {
                Id = 2,
                EmailAddress = "Bob.User@email.com",
                PhoneNumber = "1234567890",
                Name = "Bob.User",
                OrganisationId = 2,
            },
            Status = new StatusDto
            {
                Id = 1,
                Name = "New",
                SortOrder = 1
            },
            ServiceDto = new ServiceDto
            {
                Id = 2,
                Name = "Services",
                Description = "Services Description",
                OrganisationDto = new OrganisationDto
                {
                    Id = 2,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            },
            TeamDto = new TeamDto
            { 
                Id = 2,
                Name = "Team",
                OrganisationId = 2
            }

        };
    }
}
