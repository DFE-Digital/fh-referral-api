using AutoMapper;
using FamilyHubs.ServiceDirectoryCaseManagement.Core;
using System.Reflection;
using MediatR;
using FamilyHubs.SharedKernel.Interfaces;
using FamilyHubs.ServiceDirectoryCaseManagement.Infra.Service;

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
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        return services;
    }
}
