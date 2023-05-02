namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface ISetReferralStatusCommand
{
    string Status { get; }
    long ReferralId { get; }
}
