﻿using AutoMapper;
using FamilyHubs.ReferralApi.Data.Entities;
using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.ReferralApi.Core;

public class AutoMappingProfiles : Profile
{
    public AutoMappingProfiles()
    {
        CreateMap<ReferralDto, Referral>()
            .ForMember(dest => dest.Referrer, opt => opt.MapFrom(src => src.ReferrerDto))
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.RecipientDto))
            .ForMember(dest => dest.ReferralService, opt => opt.MapFrom(src => src.ReferralServiceDto))
            .ReverseMap();
        CreateMap<ReferralStatusDto, ReferralStatus>().ReverseMap();
        CreateMap<ReferrerDto, Referrer>().ReverseMap();
        CreateMap<RecipientDto, Recipient>().ReverseMap();
        CreateMap<ReferralServiceDto, Data.Entities.ReferralService>()
            .ForMember(dest => dest.ReferralOrganisation, opt => opt.MapFrom(src => src.ReferralOrganisationDto))
            .ReverseMap();
        CreateMap<ReferralOrganisationDto, ReferralOrganisation>().ReverseMap();
        CreateMap<ReferralOrganisation, ReferralOrganisation>();
    }
}
