namespace FamilyHubs.Referral.Data.Entities.Metrics;

public class ConnectionRequestsSentMetric : EntityBase<long>
{
    public required long OrganisationId { get; set; }
    public required long UserAccountId { get; set; }
    public required DateTime RequestTimestamp { get; set; }
    public required string RequestCorrelationId { get; set; } 
    public DateTime? ResponseTimestamp { get; set; }
    public byte? HttpResponseCode { get; set; }
    public long? ConnectionRequestId { get; set; }
    public string? ConnectionRequestReferenceCode { get; set; }
}