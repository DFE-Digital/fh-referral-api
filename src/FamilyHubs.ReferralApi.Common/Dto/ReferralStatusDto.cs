namespace FamilyHubs.ReferralApi.Common.Dto;

public class ReferralStatusDto
{
    private ReferralStatusDto() { }
    public ReferralStatusDto(string id, string status)
    {
        Id = id;
        Status = status;
    }

    public string Id { get; set; } = default!;
    public string Status { get; set; } = default!;
}
