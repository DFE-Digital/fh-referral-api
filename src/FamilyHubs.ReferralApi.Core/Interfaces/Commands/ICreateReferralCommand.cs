﻿using FamilyHubs.ServiceDirectory.Shared.Dto;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Commands;

public interface ICreateReferralCommand
{
    public ReferralDto ReferralDto { get; }
}
