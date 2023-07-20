using AutoMapper;
using Azure.Core;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Core.Queries.GetUserAccounts;
using FamilyHubs.Referral.Data.Models;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyHubs.Referral.Core.Commands;

public class SubscriptionValidationResponseData
{
    public string ValidationResponse { get; set; } = default!;
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
                ValidationResponse = "Event Not Handled"
            };
        }

        foreach (EventGridEventEx egEvent in egEvents)
        {
            if (egEvent.Data == null)
            {
                return new SubscriptionValidationResponseData
                {
                    ValidationResponse = "Event Not Handled"
                };

            }
            if (egEvent.EventType.IndexOf("Validation", StringComparison.OrdinalIgnoreCase) > 1)
            {
                dynamic item = egEvent.Data;
                _logger.LogInformation($"ValidationCode: {item.ValidationCode}");
                return new SubscriptionValidationResponseData
                {
                    ValidationResponse = item.ValidationCode
                };
            }
            else
            {
                await ProcessUserAccount(egEvent, cancellationToken);
            }

        }

        return new SubscriptionValidationResponseData
        {
            ValidationResponse = "Done"
        };
    }

    private EventGridEventEx[] GetEventGridEvents(ProcessUserGidEventCommand request)
    {
        EventGridEventEx[] egEvents = default!;
        using JsonDocument requestDocument = JsonDocument.Parse(request.HttpContext.Request.Body);
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
