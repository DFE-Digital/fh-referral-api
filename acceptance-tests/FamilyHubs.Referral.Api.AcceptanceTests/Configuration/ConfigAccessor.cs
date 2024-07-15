using Microsoft.Extensions.Configuration;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Configuration;

public class ConfigAccessor
{
    private static IConfigurationRoot? _root;

    private static IConfigurationRoot GetIConfigurationRoot()
    {
        return _root ??= new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();
    }

    public static ConfigModel GetApplicationConfiguration()
    {
        var configuration = new ConfigModel();

        var iConfig = GetIConfigurationRoot();

        iConfig.Bind(configuration);

        return configuration;
    }
}