using AutoMapper;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ServiceDirectory.Shared.Dto;

namespace FamilyHubs.ReferralApi.Core;

public class AutoMappingProfiles : Profile
{
    public AutoMappingProfiles()
    {
        CreateMap<ReferralDto, Referral>();
        CreateMap<ReferralStatusDto, ReferralStatus>();
    }
}
