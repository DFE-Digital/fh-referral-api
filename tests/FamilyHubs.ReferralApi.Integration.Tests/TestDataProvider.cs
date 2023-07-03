

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
            ReferrerDto = new ReferralUserAccountDto
            {
                Id = 2,
                EmailAddress = "Bob.Referrer@email.com",
                Name = "Bob Referrer",
                PhoneNumber = "011 222 5555",
                Role = "Social Worker",
                Team = "Social Work team North"
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
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }

        };
    }

    public static UserAccountDto GetUserAccount()
    {
        UserAccountDto userAccountDto = new UserAccountDto
        {
            EmailAddress = "FirstUser@email.com",
            Name = "First User",
            PhoneNumber = "0161 111 1111",
            Role = "Test Role",
            Team = "Test Team"
        };

        userAccountDto.OrganisationUserAccountDtos = new List<OrganisationUserAccountDto>
        {
            new OrganisationUserAccountDto
            {
                UserAccount = userAccountDto,
                Organisation = new OrganisationDto
                {
                    Id = 2,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }
        };

        return userAccountDto;
    }
}
