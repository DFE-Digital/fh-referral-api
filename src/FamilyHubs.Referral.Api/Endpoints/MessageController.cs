using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using System.Text;

namespace FamilyHubs.Referral.Api.Endpoints;


[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    
    private readonly IQueueClient _queueClient;

    public MessageController()
    {
        string serviceBusConnectionString = "<your-service-bus-connection-string>";
        string queueName = "<your-queue-name>";

        _queueClient = new QueueClient(serviceBusConnectionString, queueName);
    }
    
    
    [HttpPost]
    public async Task<IActionResult> ReceiveMessageAsync([FromBody] string messageBody)
    {
        try
        {
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await _queueClient.SendAsync(message);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while sending the message: {ex.Message}");
        }
    }

    // This method listens for messages from the Service Bus queue
    public async Task ReceiveMessageHandlerAsync()
    {
        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false
        };

        _queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

        await Task.CompletedTask;
    }
    
    private async Task ProcessMessageAsync(Message message, CancellationToken token)
    {
        // Process the received message here
        string messageBody = Encoding.UTF8.GetString(message.Body);

        // Do something with the message (e.g., save it to a database, perform some action, etc.)

        // Complete the message to remove it from the queue
        await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs args)
    {
        // Handle any exceptions that occur during the message processing
        Console.WriteLine($"An error occurred while receiving messages: {args.Exception}");

        return Task.CompletedTask;
    }
    
}
