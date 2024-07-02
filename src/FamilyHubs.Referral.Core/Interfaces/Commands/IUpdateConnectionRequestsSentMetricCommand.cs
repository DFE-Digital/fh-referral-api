using FamilyHubs.ReferralService.Shared.Dto.Metrics;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface IUpdateConnectionRequestsSentMetricCommand
{
    UpdateConnectionRequestsSentMetricDto MetricDto { get; init; }
}