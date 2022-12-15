using EncryptColumn.Core.Attribute;
using FamilyHubs.SharedKernel;
using FamilyHubs.SharedKernel.Interfaces;

namespace FamilyHubs.ReferralApi.Core.Entities;

public class Referral : EntityBase<string>, IAggregateRoot
{
    private Referral() { }
    public Referral(string id, string organisationId, string serviceId, string serviceName, string serviceDescription, string serviceAsJson, string referrer, string fullName, string hasSpecialNeeds, string email, string phone, string reasonForSupport, ICollection<ReferralStatus> status)
    {
        Id = id;
        OrganisationId = organisationId;
        ServiceId = serviceId;
        ServiceName = serviceName;
        ServiceDescription = serviceDescription;
        ServiceAsJson = serviceAsJson;
        Referrer = referrer;
        FullName = fullName;
        HasSpecialNeeds = hasSpecialNeeds;
        Email = email;
        Phone = phone;
        ReasonForSupport = reasonForSupport;
        Status = status;
    }
    public string OrganisationId { get; set; } = default!;
    public string ServiceId { get; set; } = default!;
    public string ServiceName { get; set; } = default!;
    public string ServiceDescription { get; set; } = default!;
    public string ServiceAsJson { get; set; } = default!;
    public string Referrer { get; set; } = default!;
    public string FullName { get; set; } = default!;
    [EncryptColumn]
    public string HasSpecialNeeds { get; set; } = default!;
    [EncryptColumn]
    public string? Email { get; set; } = default!;
    [EncryptColumn]
    public string? Phone { get; set; } = default!;
    [EncryptColumn]
    public string? Text { get; set; } = default!;
    [EncryptColumn]
    public string ReasonForSupport { get; set; } = default!;
    [EncryptColumn]
    public string? ReasonForRejection { get; set; } = default!;
    public virtual ICollection<ReferralStatus> Status { get; set; } = default!;

}
