using AutoMapper;
using FamilyHubs.ReferralApi.Common.Dto;
using FamilyHubs.ReferralApi.Core.Entities;

namespace FamilyHubs.ReferralApi.Core;

public class AutoMappingProfiles : Profile
{
    public AutoMappingProfiles()
    {
        CreateMap<ReferralDto, Referral>();
        CreateMap<ReferralStatusDto, ReferralStatus>();
    }
}
