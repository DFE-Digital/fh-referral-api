using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text;

namespace FamilyHubs.Referral.Data.Repository;

[DisallowConcurrentExecution]
public class ObfuscateHistoricalDataJob : IJob
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ObfuscateHistoricalDataJob> _logger;
    private static Random random = new Random();
    public ObfuscateHistoricalDataJob(ApplicationDbContext applicationDbContext, IConfiguration configuration, ILogger<ObfuscateHistoricalDataJob> logger)
    {
        _applicationDbContext = applicationDbContext;
        _configuration = configuration;
        _logger = logger;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            int days = _configuration.GetValue<int>("ObfuscatePIIDataInDays");
            var recipients = _applicationDbContext.Recipients.Where(r => r.Created < DateTime.UtcNow.AddDays(days) && !r.Name.Contains("Obfuscated")).ToList();

            if (recipients != null && recipients.Any())
            {
                foreach (var recipient in recipients)
                {
                    recipient.Name = RandomString(12);
                    recipient.Email = $"{RandomString(10)}.email.com";
                    recipient.Telephone = RandomString(12);
                    recipient.TextPhone = RandomString(12);
                    recipient.AddressLine1 = RandomString(12);
                    recipient.AddressLine2 = RandomString(12);
                    recipient.TownOrCity = RandomString(12);
                    recipient.County = RandomString(12);
                    recipient.PostCode = RandomString(8);
                }

                await _applicationDbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "An error occurred obfuscating referral data. {exceptionMessage}", ex.Message);
        }
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder sb = new StringBuilder();
        sb.Append("Obfuscated - ");
        sb.Append(new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray()));
       return sb.ToString();
    }
}
