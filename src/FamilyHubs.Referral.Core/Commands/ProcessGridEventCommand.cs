using AutoMapper;
using Azure.Messaging.EventGrid.SystemEvents;
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
    private readonly IMapper _mapper;
    private readonly ILogger<ProcessUserGridEventCommandHandler> _logger;

    public ProcessUserGridEventCommandHandler(ApplicationDbContext context, ISender mediator, IMapper mapper, ILogger<ProcessUserGridEventCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
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

                if (!handled)
                {
                    _logger.LogWarning("Unknown Event Data Property Type");
                    return new SubscriptionValidationResponseData
                    {
                        validationResponse = "Unknown Event Data Property Type"
                    };
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

            _logger.LogInformation($"Creating User Account for Processing Events: {userAccountDto.Name}-{userAccountDto.EmailAddress}");

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
            _logger.LogError("Event Grid Receive Message - Failed to save organisation");
            _logger.LogError(ex, ex.Message);
        }
    }

    private async Task ProcessOrganisation(OrganisationDto organisationDto, CancellationToken cancellationToken)
    {
        try
        {
            //Process Custom Event
            _logger.LogInformation($"Creating Organisation for Processing Events: {organisationDto.Name}");

            var mappedOrganisation = _mapper.Map<Organisation>(organisationDto);

            Organisation? organisation = _context.Organisations.FirstOrDefault(x => x.Id == organisationDto.Id);
            if (organisation != null)
            {
                organisation = mappedOrganisation ?? organisation;
            }
            else
            {
                _context.Organisations.Add(mappedOrganisation);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) 
        {
            _logger.LogError("Event Grid Receive Message - Failed to save organisation");
            _logger.LogError(ex, ex.Message);
        }

    }
}
