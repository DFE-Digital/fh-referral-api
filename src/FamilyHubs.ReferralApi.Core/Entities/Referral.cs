using EncryptColumn.Core.Attribute;
using System.Diagnostics.CodeAnalysis;

namespace FamilyHubs.ReferralApi.Core.Entities;

public class Referral : EntityBase<string>
{
    private Referral() { }
    [SuppressMessage("SonarLint", "S107", Justification = "Ignored intentionally as this is a database object")]
    public Referral(string id, string organisationId, string serviceId, string serviceName, string serviceDescription, string serviceAsJson, string referrer, string fullName, string hasSpecialNeeds, string? email, string? phone, string? text, string reasonForSupport, string? reasonForRejection, ICollection<ReferralStatus> status)
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
        Text = text;
        ReasonForSupport = reasonForSupport;
        ReasonForRejection = reasonForRejection;
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
