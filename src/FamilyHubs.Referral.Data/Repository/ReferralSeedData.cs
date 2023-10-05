using FamilyHubs.Referral.Data.Entities;

namespace FamilyHubs.Referral.Data.Repository;

public static class ReferralSeedData
{
#if SeedJasonService
    const string JsonService = "{\"Id\":\"ba1cca90-b02a-4a0b-afa0-d8aed1083c0d\",\"Name\":\"Test County Council\",\"Description\":\"Test County Council\",\"Logo\":null,\"Uri\":\"https://www.test.gov.uk/\",\"Url\":\"https://www.test.gov.uk/\",\"Services\":[{\"Id\":\"c1b5dd80-7506-4424-9711-fe175fa13eb8\",\"Name\":\"Test Organisation for Children with Tracheostomies\",\"Description\":\"Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.\",\"Accreditations\":null,\"Assured_date\":null,\"Attending_access\":null,\"Attending_type\":null,\"Deliverable_type\":null,\"Status\":\"active\",\"Url\":\"www.testservice.com\",\"Email\":\"support@testservice.com\",\"Fees\":null,\"ServiceDelivery\":[{\"Id\":\"14db2aef-9292-4afc-be09-5f6f43765938\",\"ServiceDelivery\":2}],\"Eligibilities\":[{\"Id\":\"Test9109Children\",\"Eligibility\":\"\",\"Maximum_age\":0,\"Minimum_age\":13}],\"Contacts\":[{\"Id\":\"5eac5cb6-cc7e-444d-a29b-76ccb85be866\",\"Title\":\"Service\",\"Name\":\"\",\"Phones\":[{\"Id\":\"1568\",\"Number\":\"01827 65779\"}]}],\"Cost_options\":[],\"Languages\":[{\"Id\":\"442a06cd-aa14-4ea3-9f11-b45c1bc4861f\",\"Language\":\"English\"}],\"Service_areas\":[{\"Id\":\"68af19cd-bc81-4585-99a2-85a2b0d62a1d\",\"Service_area\":\"National\",\"Extent\":null,\"Uri\":\"http://statistics.data.gov.uk/id/statistical-geography/K02000001\"}],\"Service_at_locations\":[{\"Id\":\"Test1749\",\"Location\":{\"Id\":\"a878aadc-6097-4a0f-b3e1-77fd4511175d\",\"Name\":\"\",\"Description\":\"\",\"Latitude\":52.6312,\"Longitude\":-1.66526,\"Physical_addresses\":[{\"Id\":\"1076aaa8-f99d-4395-8e4f-c0dde8095085\",\"Address_1\":\"75 Sheepcote Lane\",\"City\":\", Stathe, Tamworth, Staffordshire, \",\"Postal_code\":\"B77 3JN\",\"Country\":\"England\",\"State_province\":null}]}}],\"Service_taxonomys\":[{\"Id\":\"Test9107\",\"Taxonomy\":{\"Id\":\"Test bccsource:Organisation\",\"Name\":\"Organisation\",\"Vocabulary\":\"Test BCC Data Sources\",\"Parent\":null}},{\"Id\":\"Test9108\",\"Taxonomy\":{\"Id\":\"Test bccprimaryservicetype:38\",\"Name\":\"Support\",\"Vocabulary\":\"Test BCC Primary Services\",\"Parent\":null}},{\"Id\":\"Test9109\",\"Taxonomy\":{\"Id\":\"Test bccagegroup:37\",\"Name\":\"Children\",\"Vocabulary\":\"Test BCC Age Groups\",\"Parent\":null}},{\"Id\":\"Test9110\",\"Taxonomy\":{\"Id\":\"Testbccusergroup:56\",\"Name\":\"Long Term Health Conditions\",\"Vocabulary\":\"Test BCC User Groups\",\"Parent\":null}}]}]}";
#endif

    public static IReadOnlyCollection<Role> SeedRoles()
    {
        return new List<Role>()
        {
            new Role
            {
                Id = 1,
                Name = "DfeAdmin",
                Description = "DfE Administrator"
            },
            new Role
            {
                Id = 2,
                Name = "LaManager",
                Description = "Local Authority Manager"
            },
            new Role
            {
                Id = 3,
                Name = "VcsManager",
                Description = "VCS Manager"
            },
            new Role
            {
                Id = 4,
                Name = "LaProfessional",
                Description = "Local Authority Professional"
            },
            new Role
            {
                Id = 5,
                Name = "VcsProfessional",
                Description = "VCS Professional"
            },
            new Role
            {
                Id = 6,
                Name = "LaDualRole",
                Description = "Local Authority Dual Role"
            },
            new Role
            {
                Id = 7,
                Name = "VcsDualRole",
                Description = "VCS Dual Role"
            },
        };
    }

    public static IReadOnlyCollection<Status> SeedStatuses()
    {
        return new HashSet<Status>()
        {
            new Status()
            {
                Id = 1,
                Name = "New",
                SortOrder = 0,
                SecondrySortOrder = 1,
            },
            new Status()
            {
                Id = 2,
                Name = "Opened",
                SortOrder = 1,
                SecondrySortOrder = 1,
            },
            new Status()
            {
                Id = 3,
                Name = "Accepted",
                SortOrder = 2,
                SecondrySortOrder = 2,
            },
            new Status()
            {
                Id = 4,
                Name = "Declined",
                SortOrder = 3,
                SecondrySortOrder = 0,
            },
        };
    }

    public static IReadOnlyCollection<Data.Entities.Referral> SeedReferral(bool testing = false)
    {
        List<Data.Entities.Referral> listReferrals = new()
        {
            new Data.Entities.Referral
            {
                ReferrerTelephone = "0121 555 7777",
                ReasonForSupport = "Reason For Support",
                EngageWithFamily = "Engage With Family",
                Recipient = new Recipient
                {
                    Name = "Test User",
                    Email = "TestUser@email.com",
                    Telephone = "078873456",
                    TextPhone = "078873456",
                    AddressLine1 = "Address Line 1",
                    AddressLine2 = "Address Line 2",
                    TownOrCity = "Birmingham",
                    County = "Country",
                    PostCode = "B30 2TV"
                },
                UserAccount = new UserAccount
                {
                    Id = 5,
                    EmailAddress = "Joe.Professional@email.com",
                    Name = "Joe Professional",
                    PhoneNumber = "011 222 3333",
                    Team = "Social Work team North"
                },
                Status = new Status
                {
                    Id = 1,
                    Name = "New",
                    SortOrder = 1,
                    SecondrySortOrder = 0,
                },
                ReferralService = new Data.Entities.ReferralService
                {
                    Id = 1,
                    Name = "Test Service",
                    Description = "Test Service Description",
                    Url = "www.TestService.com",
                    Organisation = new Organisation
                    {
                        Id = 1,
                        ReferralServiceId = 1,
                        Name = "Test Organisation",
                        Description = "Test Organisation Description",
                    }
                }
            },

            new Data.Entities.Referral
            {
                ReferrerTelephone = "0121 555 7777",
                ReasonForSupport = "Reason For Support 2",
                EngageWithFamily = "Engage With Family 2",
                Recipient = new Recipient
                {
                    Name = "Test User 2",
                    Email = "TestUser2@email.com",
                    Telephone = "078873457",
                    TextPhone = "078873457",
                    AddressLine1 = "User 2 Address Line 1",
                    AddressLine2 = "User 2 Address Line 2",
                    TownOrCity = "Birmingham",
                    County = "Country",
                    PostCode = "B31 2TV"
                },
                UserAccount = new UserAccount
                {
                    Id = 5,
                    EmailAddress = "Joe.Professional@email.com",
                    Name = "Joe Professional",
                    PhoneNumber = "011 222 3333",
                    Team = "Social Work team North"
                },
                Status = new Status
                {
                    Id = 1,
                    Name = "Opened",
                    SortOrder = 1,
                    SecondrySortOrder = 1,
                },
                ReferralService = new Data.Entities.ReferralService
                {
                    Id = 1,
                    Name = "Test Service",
                    Description = "Test Service Description",
                    Organisation = new Organisation
                    {
                        Id = 1,
                        ReferralServiceId = 1,
                        Name = "Test Organisation",
                        Description = "Test Organisation Description",
                    }
                }
            }
        };

        if (!testing)
        {
            listReferrals.RemoveAt(1);
        }

        return listReferrals;

    }
}
