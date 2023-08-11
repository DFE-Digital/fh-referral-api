using AutoMapper;
using Azure.Messaging.EventGrid.SystemEvents;
using FamilyHubs.Referral.Core.Commands.CreateOrUpdateOrganisation;
using FamilyHubs.Referral.Core.Commands.CreateOrUpdateService;
using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Core.Interfaces;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Models;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FamilyHubs.Referral.Core.Commands;

//Reference: https://learn.microsoft.com/en-us/azure/event-grid/receive-events

public class SubscriptionValidationResponseData
{
    public string validationResponse { get; set; } = default!;
}

public class ProcessGridEventCommand : IRequest<SubscriptionValidationResponseData>, IProcessUserGridEventCommand
{
    public ProcessGridEventCommand(HttpContext context)
    {
        HttpContext = context;
    }

    public HttpContext HttpContext { get; set; }
}
public class ProcessUserGridEventCommandHandler : IRequestHandler<ProcessGridEventCommand, SubscriptionValidationResponseData>
{
    private readonly ApplicationDbContext _context;
    private readonly ISender _mediator;
    private readonly ILogger<ProcessUserGridEventCommandHandler> _logger;

    public ProcessUserGridEventCommandHandler(ApplicationDbContext context, ISender mediator, ILogger<ProcessUserGridEventCommandHandler> logger)
    {
        _context = context;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<SubscriptionValidationResponseData> Handle(ProcessGridEventCommand request, CancellationToken cancellationToken)
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
                bool handled = false;
                _logger.LogInformation($"Event Type = {egEvent.EventType}");

                if (egEvent.EventType == "UserAccountDto")
                {
                    UserAccountDto? userAccountDto = Deserialize<UserAccountDto>(egEvent.Data.ToString());
                    if (userAccountDto != null)
                    {
                        handled = true;
                        await ProcessUserAccount(userAccountDto, cancellationToken);
                    }
                }

                if (egEvent.EventType == "OrganisationDto")
                {
                    OrganisationDto? organisationDto = Deserialize<OrganisationDto>(egEvent.Data.ToString());
                    if (organisationDto != null)
                    {
                        handled = true;
                        await ProcessOrganisation(organisationDto, cancellationToken);
                    }
                }

                if (egEvent.EventType == "ReferralServiceDto")
                {
                    ReferralServiceDto? serviceDto = Deserialize<ReferralServiceDto>(egEvent.Data.ToString());
                    if (serviceDto != null)
                    {
                        handled = true;
                        await ProcessService(serviceDto, cancellationToken);
                    }
                }

                if (!handled)
                {
                    _logger.LogWarning("Unknown Event Data Property Type");
                    throw new ArgumentException($"Event Type {egEvent.EventType} did not match any known types");
                }
            }

        }

        return new SubscriptionValidationResponseData
        {
            validationResponse = "All Done!"
        };
    }

    private EventGridEventEx[] GetEventGridEvents(ProcessGridEventCommand request)
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

    private T Deserialize<T>(string? json)
    {
        try 
        {
            if (string.IsNullOrEmpty(json))
                return default!;

            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            if (item == null)
            {
                return default!;
            }

            return item;
        }
        catch
        {
            return default!;
        }
    }

    private async Task ProcessUserAccount(UserAccountDto userAccountDto, CancellationToken cancellationToken)
    {
        try
        {
            //Process Custom Event
            _logger.LogInformation("Processing User Account Event Grid event message");

            _logger.LogInformation("Creating User Account for Processing Events: {UserAccountName}-{UserAccountEmailAddress}", userAccountDto.Name,userAccountDto.EmailAddress);

            var user = _context.UserAccounts.FirstOrDefault(x => x.Id == userAccountDto.Id);
            if (user != null)
            {
                UpdateUserAccountCommand updateUserAccountCommand = new UpdateUserAccountCommand(userAccountDto.Id, userAccountDto);
                await _mediator.Send(updateUserAccountCommand, cancellationToken);
            }
            else
            {
                //Will have throw a NotFoundException so need to add
                CreateUserAccountCommand command = new(userAccountDto);
                await _mediator.Send(command, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Event Grid Receive Message - Failed to save Account" );
        }
    }

    private async Task ProcessOrganisation(OrganisationDto organisationDto, CancellationToken cancellationToken)
    {
        CreateOrUpdateOrganisationCommand createOrUpdateOrganisationCommand = new(organisationDto);
        await _mediator.Send(createOrUpdateOrganisationCommand, cancellationToken);
    }

    private async Task ProcessService(ReferralServiceDto referralServiceDto, CancellationToken cancellationToken)
    {
        CreateOrUpdateServiceCommand createOrUpdateOrganisationCommand = new(referralServiceDto);
        await _mediator.Send(createOrUpdateOrganisationCommand, cancellationToken);
    }
}
