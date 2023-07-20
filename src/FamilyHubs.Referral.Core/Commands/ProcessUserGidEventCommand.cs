using Azure.Messaging.EventGrid.SystemEvents;
using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Data.Models;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FamilyHubs.Referral.Core.Commands;

//Reference: https://learn.microsoft.com/en-us/azure/event-grid/receive-events

public class SubscriptionValidationResponseData
{
    public string validationResponse { get; set; } = default!;
}

public class ProcessUserGidEventCommand : IRequest<SubscriptionValidationResponseData>//, IProcessUserGidEventCommand
{
    public ProcessUserGidEventCommand(HttpContext context)
    {
        HttpContext = context;
    }

    public HttpContext HttpContext { get; set; }
}
public class ProcessUserGidEventCommandHandler : IRequestHandler<ProcessUserGidEventCommand, SubscriptionValidationResponseData>
{
    private readonly ApplicationDbContext _context;
    private readonly ISender _mediator;
    private readonly ILogger<ProcessUserGidEventCommandHandler> _logger;

    public ProcessUserGidEventCommandHandler(ApplicationDbContext context, ISender mediator, ILogger<ProcessUserGidEventCommandHandler> logger)
    {
        _context = context;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<SubscriptionValidationResponseData> Handle(ProcessUserGidEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calling MinimalUserAccountEndPoints (Process Azure Grid Events)");

        var syncIOFeature = request.HttpContext.Features.Get<IHttpBodyControlFeature>();
        if (syncIOFeature != null)
        {
            syncIOFeature.AllowSynchronousIO = true;
        }

        EventGridEventEx[] egEvents = GetEventGridEvents(request);

        if (egEvents == null)
        {
            return new SubscriptionValidationResponseData
            {
                validationResponse = "Event Not Handled"
            };
        }

        foreach (EventGridEventEx egEvent in egEvents)
        {
            if (egEvent.Data == null)
            {
                return new SubscriptionValidationResponseData
                {
                    validationResponse = "Event Not Handled"
                };

            }
            if (egEvent.EventType.IndexOf("Validation", StringComparison.OrdinalIgnoreCase) > 1)
            {
                if (egEvent.Data is SubscriptionValidationEventData)
                {
                    SubscriptionValidationEventData? subscriptionValidationEventData = egEvent.Data as SubscriptionValidationEventData;
                    if (subscriptionValidationEventData != null)
                    {
                        _logger.LogInformation($"Using SubscriptionValidationEventData - ValidationCode: {subscriptionValidationEventData.ValidationCode}");
                        return new SubscriptionValidationResponseData
                        {
                            validationResponse = subscriptionValidationEventData.ValidationCode
                        };
                    }
                }

                dynamic item = egEvent.Data;
                _logger.LogInformation($"Using dynamic type - ValidationCode: {item.validationCode}");
                return new SubscriptionValidationResponseData
                {
                    validationResponse = item.validationCode
                };
            }
            else
            {
                await ProcessUserAccount(egEvent, cancellationToken);
            }

        }

        return new SubscriptionValidationResponseData
        {
            validationResponse = "All Done!"
        };
    }

    private EventGridEventEx[] GetEventGridEvents(ProcessUserGidEventCommand request)
    {
        EventGridEventEx[] egEvents = default!;
        using JsonDocument requestDocument = JsonDocument.Parse(request.HttpContext.Request.Body);
        _logger.LogInformation("Payload - " + requestDocument.RootElement.ToString());
        if (requestDocument.RootElement.ValueKind == JsonValueKind.Object)
        {
            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<EventGridEventEx>(requestDocument.RootElement.ToString());
            if (item != null)
            {
                egEvents = new EventGridEventEx[1];
                egEvents[0] = item;

            }
        }
        else if (requestDocument.RootElement.ValueKind == JsonValueKind.Array)
        {
            egEvents = new EventGridEventEx[requestDocument.RootElement.GetArrayLength()];
            int i = 0;
            foreach (JsonElement property in requestDocument.RootElement.EnumerateArray())
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<EventGridEventEx>(property.ToString());
                if (item != null)
                {
                    egEvents[i++] = item;
                }
            }
        }

        return egEvents;
    }

    private async Task ProcessUserAccount(EventGridEventEx egEvent, CancellationToken cancellationToken)
    {
        //Process Custom Event
        _logger.LogInformation("Handling the Custom Event Grid event message");

        var userAccount = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccountDto>(egEvent?.Data?.ToString() ?? string.Empty);

        if (userAccount != null)
        {
            _logger.LogInformation($"Creating User Account for Processing Events: {userAccount.Name}-{userAccount.EmailAddress}");

            try
            {
                var user = _context.UserAccounts.FirstOrDefault(x => x.Id == userAccount.Id);
                if (user != null)
                {
                    UpdateUserAccountCommand updateUserAccountCommand = new UpdateUserAccountCommand(userAccount.Id, userAccount);
                    await _mediator.Send(updateUserAccountCommand, cancellationToken);
                }
            }
            catch
            {
                //Will have throw a NotFoundException so need to add
                CreateUserAccountCommand command = new(userAccount);
                await _mediator.Send(command, cancellationToken);
            }
        }
    }
}
