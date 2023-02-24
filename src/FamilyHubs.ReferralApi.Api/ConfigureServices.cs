using AutoMapper;
using FamilyHubs.ReferralApi.Core;
using FamilyHubs.ReferralApi.Infrastructure.Service;
using FamilyHubs.SharedKernel.Interfaces;
using MediatR;
using System.Reflection;

namespace FamilyHubs.ReferralApi.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMappingProfiles());
        });

        var mapper = config.CreateMapper();

        services.AddSingleton(mapper);

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddMediatR(Assembly.GetExecutingAssembly());

        return services;
    }
}
