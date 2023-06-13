using FamilyHubs.Referral.Data.Repository;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

#if ISLOCAL

namespace FamilyHubs.Referral.HistoricalData.Tests
{
    public class WhenUsingHistoricalData : BaseHistoricalData
    {
        [Fact]
        public void ThenTestHistoricalData()
        {
            using var scope = _webAppFactory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var context = scopedServices.GetRequiredService<ApplicationDbContext>();

            int milliseconds = 8000;
            Thread.Sleep(milliseconds);
            
            var recipients = context.Recipients.Where(r => r.Created < DateTime.UtcNow.AddDays(-2556)).ToList();

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
}

#endif