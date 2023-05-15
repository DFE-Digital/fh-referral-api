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
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.TeamDto))
            .ForMember(dest => dest.Referrer, opt => opt.MapFrom(src => src.UserDto))
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.RecipientDto))
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.ServiceDto))
            .ReverseMap()
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified));
        CreateMap<StatusDto, Status>().ReverseMap();
        CreateMap<RoleDto, Role>().ReverseMap();
        CreateMap<TeamDto, Team>().ReverseMap();
        CreateMap<UserDto, User>().ReverseMap();
        CreateMap<RecipientDto, Recipient>().ReverseMap();
        CreateMap<ServiceDto, Data.Entities.Service>()
            .ForMember(dest => dest.Organisation, opt => opt.MapFrom(src => src.OrganisationDto))
            .ReverseMap();
        CreateMap<OrganisationDto, Organisation>().ReverseMap();
        CreateMap<Organisation, Organisation>();
    }
}
