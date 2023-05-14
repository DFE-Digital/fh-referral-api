using AutoMapper;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.Referral.Core;

public class AutoMappingProfiles : Profile
{
    public AutoMappingProfiles()
    {
        CreateMap<ReferralDto, Data.Entities.Referral>()
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.LastModified, opt => opt.Ignore())
            .ForMember(dest => dest.Referrer, opt => opt.MapFrom(src => src.ReferrerDto))
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.RecipientDto))
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.ReferralServiceDto))
            .ReverseMap()
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified));
        //CreateMap<ReferralStatusDto, ReferralStatus>().ReverseMap();
        CreateMap<ReferrerDto, User>().ReverseMap();
        CreateMap<RecipientDto, Recipient>().ReverseMap();
        CreateMap<ReferralServiceDto, Data.Entities.Service>()
            .ForMember(dest => dest.Organisation, opt => opt.MapFrom(src => src.ReferralOrganisationDto))
            .ReverseMap();
        CreateMap<ReferralOrganisationDto, Organisation>().ReverseMap();
        CreateMap<Organisation, Organisation>();
    }
}
