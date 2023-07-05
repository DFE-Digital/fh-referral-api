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
            .ForMember(dest => dest.UserAccount, opt => opt.MapFrom(src => src.ReferralUserAccountDto))
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.RecipientDto))
            .ForMember(dest => dest.ReferralService, opt => opt.MapFrom(src => src.ReferralServiceDto))
            .ReverseMap()
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified));
        CreateMap<ReferralStatusDto, Status>().ReverseMap();
        CreateMap<RecipientDto, Recipient>().ReverseMap();
        CreateMap<ReferralServiceDto, Data.Entities.ReferralService>()
            .ForMember(dest => dest.Organisation, opt => opt.MapFrom(src => src.OrganisationDto))
            .ReverseMap()
            .ForMember(dest => dest.OrganisationDto, opt => opt.MapFrom(src => src.Organisation));

        // Organisation mappings
        CreateMap<OrganisationDto, Organisation>()
            .ReverseMap();

        // UserAccount mappings
        CreateMap<UserAccountDto, UserAccount>()
            .ForMember(dest => dest.OrganisationUserAccounts, opt => opt.MapFrom(src => src.OrganisationUserAccounts))
            .ReverseMap()
            .ForMember(dest => dest.OrganisationUserAccounts, opt => opt.MapFrom(src => src.OrganisationUserAccounts));

        CreateMap<RoleDto, Role>()
            .ReverseMap();
        CreateMap<UserAccountRoleDto, UserAccountRole>()
            .ReverseMap();

        CreateMap<UserAccountServiceDto, UserAccountService>()
            .ReverseMap();

        // OrganisationUserAccount mappings
        CreateMap<UserAccountOrganisationDto, UserAccountOrganisation>()
            .ForMember(dest => dest.Organisation, opt => opt.MapFrom(src => src.Organisation))
            .ForMember(dest => dest.UserAccount, opt => opt.MapFrom(src => src.UserAccount))
            .ReverseMap()
            .ForMember(dest => dest.Organisation, opt => opt.MapFrom(src => src.Organisation))
            .ForMember(dest => dest.UserAccount, opt => opt.MapFrom(src => src.UserAccount));

        CreateMap<Role,Role>().ReverseMap();
        
        
    }
}
