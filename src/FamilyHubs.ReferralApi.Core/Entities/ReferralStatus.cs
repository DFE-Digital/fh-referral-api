using FamilyHubs.SharedKernel;

namespace FamilyHubs.ReferralApi.Core.Entities;

public class ReferralStatus : EntityBase<string>
{
    private ReferralStatus() { }
    public ReferralStatus(string id, string status, string referralId)
    {
        Id = id;
        Status = status;
        ReferralId = referralId;
    }

    public string Status { get; set; } = default!;
    public string ReferralId { get; set; } = default!;

}
