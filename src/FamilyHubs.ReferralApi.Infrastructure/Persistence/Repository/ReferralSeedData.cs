using FamilyHubs.ReferralApi.Core.Entities;

namespace FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;

public static class ReferralSeedData
{
    const string JsonService = "{\"Id\":\"ba1cca90-b02a-4a0b-afa0-d8aed1083c0d\",\"Name\":\"Test County Council\",\"Description\":\"Test County Council\",\"Logo\":null,\"Uri\":\"https://www.test.gov.uk/\",\"Url\":\"https://www.test.gov.uk/\",\"Services\":[{\"Id\":\"c1b5dd80-7506-4424-9711-fe175fa13eb8\",\"Name\":\"Test Organisation for Children with Tracheostomies\",\"Description\":\"Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.\",\"Accreditations\":null,\"Assured_date\":null,\"Attending_access\":null,\"Attending_type\":null,\"Deliverable_type\":null,\"Status\":\"active\",\"Url\":\"www.testservice.com\",\"Email\":\"support@testservice.com\",\"Fees\":null,\"ServiceDelivery\":[{\"Id\":\"14db2aef-9292-4afc-be09-5f6f43765938\",\"ServiceDelivery\":2}],\"Eligibilities\":[{\"Id\":\"Test9109Children\",\"Eligibility\":\"\",\"Maximum_age\":0,\"Minimum_age\":13}],\"Contacts\":[{\"Id\":\"5eac5cb6-cc7e-444d-a29b-76ccb85be866\",\"Title\":\"Service\",\"Name\":\"\",\"Phones\":[{\"Id\":\"1568\",\"Number\":\"01827 65779\"}]}],\"Cost_options\":[],\"Languages\":[{\"Id\":\"442a06cd-aa14-4ea3-9f11-b45c1bc4861f\",\"Language\":\"English\"}],\"Service_areas\":[{\"Id\":\"68af19cd-bc81-4585-99a2-85a2b0d62a1d\",\"Service_area\":\"National\",\"Extent\":null,\"Uri\":\"http://statistics.data.gov.uk/id/statistical-geography/K02000001\"}],\"Service_at_locations\":[{\"Id\":\"Test1749\",\"Location\":{\"Id\":\"a878aadc-6097-4a0f-b3e1-77fd4511175d\",\"Name\":\"\",\"Description\":\"\",\"Latitude\":52.6312,\"Longitude\":-1.66526,\"Physical_addresses\":[{\"Id\":\"1076aaa8-f99d-4395-8e4f-c0dde8095085\",\"Address_1\":\"75 Sheepcote Lane\",\"City\":\", Stathe, Tamworth, Staffordshire, \",\"Postal_code\":\"B77 3JN\",\"Country\":\"England\",\"State_province\":null}]}}],\"Service_taxonomys\":[{\"Id\":\"Test9107\",\"Taxonomy\":{\"Id\":\"Test bccsource:Organisation\",\"Name\":\"Organisation\",\"Vocabulary\":\"Test BCC Data Sources\",\"Parent\":null}},{\"Id\":\"Test9108\",\"Taxonomy\":{\"Id\":\"Test bccprimaryservicetype:38\",\"Name\":\"Support\",\"Vocabulary\":\"Test BCC Primary Services\",\"Parent\":null}},{\"Id\":\"Test9109\",\"Taxonomy\":{\"Id\":\"Test bccagegroup:37\",\"Name\":\"Children\",\"Vocabulary\":\"Test BCC Age Groups\",\"Parent\":null}},{\"Id\":\"Test9110\",\"Taxonomy\":{\"Id\":\"Testbccusergroup:56\",\"Name\":\"Long Term Health Conditions\",\"Vocabulary\":\"Test BCC User Groups\",\"Parent\":null}}]}]}";
    public static IReadOnlyCollection<Referral> SeedReferral(bool addMany = false)
    {

        List<Referral> listReferrals = new();

        listReferrals.Add(

            new Referral(
                "24572563-7d73-4127-b348-8d2bf646e7fe",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr John Smith",
                "No",
                "John.Smith@yahoo.co.uk",
                "0131 111 2222",
                "0131 111 2222",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("60abfe12-be36-4d4c-ae61-d039589f7318", "Initial Connection", "24572563-7d73-4127-b348-8d2bf646e7fe") }
                )

            );
        
        if (addMany)
        {
            listReferrals.AddRange(

            new List<Referral>()
            {
                new Referral(
                "ed2b9bc0-b5cc-4420-bdb1-e518b60dedd6",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Dean Keaton",
                "No",
                "dean.keaton@email.co.uk",
                "0131 111 2223",
                "0131 111 2223",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 2), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("d750269a-029a-42a5-9f7d-082eeeb76174", "Initial Connection", "ed2b9bc0-b5cc-4420-bdb1-e518b60dedd6") }
                ),

                new Referral(
                "03b4fd20-d6b8-4fa1-8161-f726ef1cdeab",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Fred Fenster",
                "No",
                "fred.fenster@email.co.uk",
                "0131 111 2224",
                "0131 111 2224",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 3), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("5eebbefb-eab2-4989-ba65-d94a6172bcf9", "Initial Connection", "03b4fd20-d6b8-4fa1-8161-f726ef1cdeab") }
                ),

                new Referral(
                "80c36bb6-80c5-40e5-93ef-71abe11977d6",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Jack Bear",
                "No",
                "Jack.Baer@email.co.uk",
                "0131 111 2225",
                "0131 111 2225",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 4), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("52336e08-bfe0-45ec-bb44-296fa52edd42", "Initial Connection", "80c36bb6-80c5-40e5-93ef-71abe11977d6") }
                ),

                new Referral(
                "86fa8175-efb2-41e4-95ed-fc9b752d03f8",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Dave Kujan",
                "No",
                "Dave.Kujan@email.co.uk",
                "0131 111 2226",
                "0131 111 2226",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 5), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("c8126d0b-b378-4dbd-8d43-c1debad55ee1", "Initial Connection", "86fa8175-efb2-41e4-95ed-fc9b752d03f8") }
                ),

                new Referral(
                "85f34382-caa2-4ef4-bbad-ff0118d5f8b3",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Michael McManus",
                "No",
                "Michael.McManus@email.co.uk",
                "0131 111 2227",
                "0131 111 2227",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 6), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("917a3d94-bb53-4929-a1d2-62584c3e3e5c", "Initial Connection", "85f34382-caa2-4ef4-bbad-ff0118d5f8b3") }
                ),

                new Referral(
                "6dd2b090-c982-47fc-9350-b89839271931",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Tod Hockney",
                "No",
                "Tod.Hockney@email.co.uk",
                "0131 111 2228",
                "0131 111 2228",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 7), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("b526cc05-97ce-4d1f-9a3e-976f9e06de95", "Initial Connection", "6dd2b090-c982-47fc-9350-b89839271931") }
                ),

                new Referral(
                "61f0b1de-cdc5-445e-bd3b-448595d2015e",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Edie Finneran",
                "No",
                "Edie.Finneran@email.co.uk",
                "0131 111 2229",
                "0131 111 2229",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 8), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("2d00b6f7-c3ec-4ab6-8a95-3e5eee37c8a7", "Initial Connection", "61f0b1de-cdc5-445e-bd3b-448595d2015e") }
                ),

                new Referral(
                "74507e0f-3151-495f-a875-cb7eab8450d1",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Arkosh Kovash",
                "No",
                "Arkosh.Kovash@email.co.uk",
                "0131 111 2210",
                "0131 111 2210",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 9), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("2b0b69e9-1e95-4599-9bb7-1db2cff465a2", "Initial Connection", "74507e0f-3151-495f-a875-cb7eab8450d1") }
                ),

                new Referral(
                "c4f7bf19-b61b-4107-98ac-a815c0dd1e5d",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Saul Berg",
                "No",
                "Saul.Berg@email.co.uk",
                "0131 111 2211",
                "0131 111 2211",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 10), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("89312958-7bda-4d7f-997d-cdcd8514ad61", "Initial Connection", "c4f7bf19-b61b-4107-98ac-a815c0dd1e5d") }
                ),

                new Referral(
                "9a3f6a5d-5ffe-477f-8ce8-d5ca33288e1b",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Roger Kint",
                "No",
                "Roger.Kint@email.co.uk",
                "0131 111 2212",
                "0131 111 2212",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 11), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("a19587bd-0a9c-498b-9361-a44fb4b36129", "Initial Connection", "9a3f6a5d-5ffe-477f-8ce8-d5ca33288e1b") }
                ),

                new Referral(
                "364dabaa-3ce2-4e1e-accb-48def7489925",
                "72e653e8-1d05-4821-84e9-9177571a6013",
                "4591d551-0d6a-4c0d-b109-002e67318231",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "BtlPro@email.com",
                "Mr Daniel Metzheiser",
                "No",
                "Daniel.Metzheiser@email.co.uk",
                "0131 111 2214",
                "0131 111 2214",
                "Requires help with child",
                null,
                DateTime.SpecifyKind(new DateTime(2023, 1, 12), DateTimeKind.Utc),
                new List<ReferralStatus> { new ReferralStatus("62386691-9cc6-4726-a714-8eb47baa0430", "Initial Connection", "364dabaa-3ce2-4e1e-accb-48def7489925") }
                ),
            }

            

            );
        }

        for(int i = 0; i < listReferrals.Count; i++)
        {
            listReferrals[i].RequestNumber = (i + 1);
        }

        

        return listReferrals;

    }
}
