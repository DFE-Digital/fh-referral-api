using System.Diagnostics;

namespace FamilyHubs.Referral.Api.Endpoints;

public class MinimalGeneralEndPoints
{
    public void RegisterMinimalGeneralEndPoints(WebApplication app)
    {
        app.MapGet("api/info", (ILogger<MinimalGeneralEndPoints> logger) =>
        {
            logger.LogInformation("api/info called");
            var assembly = typeof(MinimalGeneralEndPoints).Assembly;

            var creationDate = File.GetCreationTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Results.Ok($"Version: {version}, Last Updated: {creationDate}");

        });
    }
}

