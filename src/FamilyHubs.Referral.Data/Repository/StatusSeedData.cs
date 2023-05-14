using FamilyHubs.Referral.Data.Entities;

namespace FamilyHubs.Referral.Data.Repository;

public static class StatusSeedData
{
    public static IReadOnlyCollection<Entities.Status> SeedStatuses()
    {
        return new HashSet<Entities.Status>()
        {
            new Status()
            {
                Name = "New",
                SortOrder = 0
            },
            new Status()
            {
                Name = "Opened",
                SortOrder = 1
            },
            new Status()
            {
                Name = "Accepted",
                SortOrder = 2
            },
            new Status()
            {
                Name = "Declined",
                SortOrder = 3
            },
        };
    }
}
