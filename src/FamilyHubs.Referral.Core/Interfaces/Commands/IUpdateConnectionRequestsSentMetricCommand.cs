
using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface IUpdateConnectionRequestsSentMetricCommand
{
    UpdateConnectionRequestsSentMetricDto MetricDto { get; init; }
}