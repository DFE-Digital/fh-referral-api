using FamilyHubs.Referral.Data.Repository;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;

namespace FamilyHubs.Referral.UnitTests;

public class WhenObfuscatingReferralData : BaseCreateDbUnitTest
{
    [Fact]
    public async Task ThenReferralDataIsObfuscated()
    {
        //Arrange
        var myConfiguration = new Dictionary<string, string?>
        {
            {"ObfuscatePIIDataInDays", "-2555"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();

        Mock<IJobExecutionContext> jobExecutionContext = new Mock<IJobExecutionContext>();
        var logger = new Mock<ILogger<ObfuscateHistoricalDataJob>>();
        var mockApplicationDbContext = GetApplicationDbContext();
        await CreateReferrals(mockApplicationDbContext);
        foreach (var item in mockApplicationDbContext.Recipients) 
        {
            item.Created = DateTime.UtcNow.AddDays(-2556);
        }
        mockApplicationDbContext.SaveChanges();

        ObfuscateHistoricalDataJob obfuscateHistoricalDataJob = new ObfuscateHistoricalDataJob(mockApplicationDbContext, configuration, logger.Object);

        await obfuscateHistoricalDataJob.Execute(jobExecutionContext.Object);

        var recipients = mockApplicationDbContext.Recipients.Where(r => r.Created < DateTime.UtcNow.AddYears(-7)).ToList();

        if (recipients.Any())
        {
            foreach (var recipient in recipients)
            {
                if (recipient == null)
                    continue;

                bool isObfuscated = recipient.Name?.Contains("Obfuscated") ?? false;
                isObfuscated = isObfuscated &&
                IsObfuscated(recipient.Email) &&
                IsObfuscated(recipient.Telephone) &&
                IsObfuscated(recipient.TextPhone) &&
                IsObfuscated(recipient.AddressLine1) &&
                IsObfuscated(recipient.AddressLine2) &&
                IsObfuscated(recipient.TownOrCity) &&
                IsObfuscated(recipient.County) &&
                IsObfuscated(recipient.PostCode);

                isObfuscated.Should().BeTrue();

            }
        }
        else
        {
            bool isObfuscated = false;
            isObfuscated.Should().BeTrue();
        }

    }

    private bool IsObfuscated(string? value)
    {
        if (value == null)
            return true;

        return value.Contains("Obfuscated");
    }
}
