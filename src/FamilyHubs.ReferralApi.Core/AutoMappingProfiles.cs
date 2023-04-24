using AutoMapper;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;

namespace FamilyHubs.ReferralApi.Core;

public class AutoMappingProfiles : Profile
{
    public AutoMappingProfiles()
    {
        CreateMap<ReferralDto, Referral>().ReverseMap();
        CreateMap<ReferralStatusDto, ReferralStatus>().ReverseMap();
        CreateMap<ReferrerDto, Referrer>().ReverseMap();
        CreateMap<RecipientDto, Recipient>().ReverseMap();
        CreateMap<ReferralServiceDto, ReferralService>().ReverseMap();
        CreateMap<ReferralOrganisationDto, ReferralOrganisation>().ReverseMap();
    }
}
