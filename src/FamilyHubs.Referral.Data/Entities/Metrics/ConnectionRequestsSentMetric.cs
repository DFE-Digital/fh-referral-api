namespace FamilyHubs.Referral.Data.Entities.Metrics;

public class ConnectionRequestsSentMetric : EntityBase<long>
{
    public long OrganisationId { get; set; }
    public long UserAccountId { get; set; }
    public DateTime ConnectionRequestSentTimestamp { get; set; }
    public Guid CorrelationId { get; set; }
    public long? ConnectionRequestId { get; set; }
    public string? ConnectionRequestReference { get; set; }
    public DateTime? ConnectionRequestReferenceSetTimestamp { get; set; }
}