﻿namespace FamilyHubs.Referral.Data.Entities.Metrics;

public class ConnectionRequestsSentMetric : EntityBase<long>
{
    public required long OrganisationId { get; init; }
    public required long UserAccountId { get; init; }
    public required DateTime RequestTimestamp { get; init; }
    public required string RequestCorrelationId { get; init; } 
    public DateTime? ResponseTimestamp { get; set; }
    public byte? HttpResponseCode { get; set; }
    public long? ConnectionRequestId { get; set; }
    public string? ConnectionRequestReferenceCode { get; set; }
}