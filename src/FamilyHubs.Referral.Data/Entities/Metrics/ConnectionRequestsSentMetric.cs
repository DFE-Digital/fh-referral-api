namespace FamilyHubs.Referral.Data.Entities.Metrics;

public class ConnectionRequestsSentMetric : EntityBase<long>
{
    public long OrganisationId { get; set; }
    public long UserAccountId { get; set; }
    public DateTime RequestTimestamp { get; set; }
    public string RequestCorrelationId { get; set; }
    public DateTime? ResponseTimestamp { get; set; }
    public byte? HttpResponseCode { get; set; }
    public long? ConnectionRequestId { get; set; }
    public string? ConnectionRequestReferenceCode { get; set; }
}