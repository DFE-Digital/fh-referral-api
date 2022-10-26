using FamilyHubs.SharedKernel;
using FamilyHubs.SharedKernel.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FamilyHubs.ReferralApi.Core.Entities;

public class Referral : EntityBase<string>, IAggregateRoot
{
    private Referral() { }
    public Referral(string id, string organisationId, string serviceId, string serviceName, string serviceDescription, string serviceAsJson, string referrer, string fullName, string? hasSpecialNeeds, string? email, string? phone, string? text, string reasonForSupport, ICollection<ReferralStatus> status)
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
        Status = status;
    }
    public string OrganisationId { get; set; } = default!;
    public string ServiceId { get; set; } = default!;
    public string ServiceName { get; set; } = default!;
    public string ServiceDescription { get; set; } = default!;
    public string ServiceAsJson { get; set; } = default!;
    public string Referrer { get; set; } = default!;
    [MaxLength(255)]
    public string FullName { get; set; } = default!;
    public string? HasSpecialNeeds { get; set; } = default!;
    [EmailAddress]
    public string? Email { get; set; } = default!;
    [Phone]
    public string? Phone { get; set; } = default!;
    public string? Text { get; set; } = default!;
    [MaxLength(1000)]
    public string ReasonForSupport { get; set; } = default!;
    public virtual ICollection<ReferralStatus> Status { get; set; } = default!;

}
