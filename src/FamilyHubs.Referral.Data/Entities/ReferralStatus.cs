﻿namespace FamilyHubs.Referral.Data.Entities;

public class ReferralStatus : EntityBase<long>
{
    public required string Name { get; set; } = default!;
    public byte SortOrder { get; set; }

}
