using FamilyHubs.ReferralApi.Common.Dto;
using FamilyHubs.ReferralApi.Common.MassTransit;
using MassTransit;
using MediatR;
using System.Text.Json;

namespace FamilyHubs.ReferralApi.Api;

public class CommandMessageConsumer : IConsumer<CommandMessage>
{
    public async Task Consume(ConsumeContext<CommandMessage> context)
    {
        var message = context.Message;
        await Console.Out.WriteLineAsync($"Message from Producer : {message.MessageString}");

        using (var scope = Program.ServiceProvider.CreateScope())
        {
            ILogger<CommandMessageConsumer>? logger = null;

            try
            {
                logger = scope.ServiceProvider.GetService<ILogger<CommandMessageConsumer>>();
                if (context != null && context.Message != null && !string.IsNullOrEmpty(context.Message.MessageString))
                {
                    ReferralDto? dto = JsonSerializer.Deserialize<ReferralDto>(context.Message.MessageString, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (dto != null)
                    {
                        CreateReferralCommand command = new(dto);
                        var mediator = scope.ServiceProvider.GetService<ISender>();
                        if (mediator != null)
                        {
                            var result = await mediator.Send(command, new CancellationToken());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (logger != null)
                {
                    logger.LogError(ex, "An error occurred consumming message.");
                }

                throw;
            }
        }
    }
}
