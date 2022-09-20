
using FamilyHubs.ReferralApi.Common.Dto;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Commands;

public interface ICreateReferralCommand
{
    public ReferralDto ReferralDto { get; }
}
