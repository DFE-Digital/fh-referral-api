namespace FamilyHubs.ReferralApi.Core.Interfaces.Commands;

public interface ISetReferralStatusCommand
{
    string Status { get; }
    string ReferralId { get; }
}
