using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.SharedKernel.Identity.Models;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface IUpdateConnectionRequestsSentMetricCommand
{
    UpdateConnectionRequestsSentMetricDto MetricDto { get; }
    FamilyHubsUser FamilyHubsUser { get; }
}