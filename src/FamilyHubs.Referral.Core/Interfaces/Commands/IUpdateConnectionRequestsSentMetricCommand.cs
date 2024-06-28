
namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface IUpdateConnectionRequestsSentMetricCommand
{
    byte HttpResponseCode { get; init; }
    long ConnectionRequestId { get; init; }
}