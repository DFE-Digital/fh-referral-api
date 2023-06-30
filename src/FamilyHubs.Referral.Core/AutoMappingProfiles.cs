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
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ReferralUserAccount, opt => opt.MapFrom(src => src.ReferrerDto))
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.RecipientDto))
            .ForMember(dest => dest.ReferralService, opt => opt.MapFrom(src => src.ReferralServiceDto))
            .ReverseMap()
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified));
        CreateMap<ReferralStatusDto, ReferralStatus>().ReverseMap();
        CreateMap<ReferralUserAccountDto, ReferralUserAccount>().ReverseMap();
        CreateMap<RecipientDto, Recipient>().ReverseMap();
        CreateMap<ReferralServiceDto, Data.Entities.ReferralService>()
            .ForMember(dest => dest.Organisation, opt => opt.MapFrom(src => src.OrganisationDto))
            .ReverseMap();

        CreateMap<UserAccountDto, UserAccount>().ReverseMap();
        CreateMap<OrganisationUserAccountDto, OrganisationUserAccount>().ReverseMap();

        CreateMap<OrganisationDto, Organisation>().ReverseMap();
        CreateMap<Organisation, Organisation>();
    }
}
