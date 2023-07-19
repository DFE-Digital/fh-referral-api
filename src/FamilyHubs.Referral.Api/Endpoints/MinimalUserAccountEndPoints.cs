using Azure.Core;
using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Core.Queries.GetUserAccounts;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection.PortableExecutable;
using FamilyHubs.Referral.Data.Entities;

namespace FamilyHubs.Referral.Api.Endpoints;
public class MinimalUserAccountEndPoints
{
    public class SubscriptionValidationResponseData
    {
        public string ValidationResponse { get; set; } = default!;
    }

    public void RegisterUserAccountEndPoints(WebApplication app)
    {
        app.MapPost("api/useraccount", [Authorize(Policy = "ReferralUser")] async ([FromBody] UserAccountDto userAccount, CancellationToken cancellationToken, ISender _mediator) =>
        {
            CreateUserAccountCommand command = new(userAccount);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("User Accounts", "Create Single User Account") { Tags = new[] { "User Accounts" } });

        app.MapPut("api/useraccount/{Id}", [Authorize(Policy = "ReferralUser")] async (long id, [FromBody] UserAccountDto userAccount, CancellationToken cancellationToken, ISender _mediator) =>
        {
            UpdateUserAccountCommand command = new(id, userAccount);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("User Accounts", "Update User Accounts") { Tags = new[] { "User Accounts" } });

        app.MapPost("api/useraccounts", [Authorize(Policy = "ReferralUser")] async ([FromBody] List<UserAccountDto> userAccounts, CancellationToken cancellationToken, ISender _mediator) =>
        {
            CreateUserAccountsCommand command = new(userAccounts);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("User Accounts", "Create User Accounts") { Tags = new[] { "User Accounts" } });

        app.MapPut("api/useraccounts", [Authorize(Policy = "ReferralUser")] async ([FromBody] List<UserAccountDto> userAccounts, CancellationToken cancellationToken, ISender _mediator) =>
        {
            UpdateUserAccountsCommand command = new(userAccounts);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("User Accounts", "Update User Accounts") { Tags = new[] { "User Accounts" } });

        app.MapGet("api/useraccountsByOrganisationId/{organisationId}", [Authorize(Policy = "ReferralUser")] async (long organisationId, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetUsersByOrganisationIdCommand command = new(organisationId, pageNumber, pageSize);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("User Accounts", "Get User Accounts By Organisation Id") { Tags = new[] { "User Accounts" } });

        

        app.MapPost("/events", async (HttpContext context, CancellationToken cancellationToken, ISender _mediator, ILogger<MinimalUserAccountEndPoints> logger) =>
        {
            logger.LogInformation("Calling MinimalUserAccountEndPoints (Process Azure Grid Events)");

            EventGridEvent? eventGridEvent = null;

            //Deserializing the request 
            var eventGridEvents = await System.Text.Json.JsonSerializer.DeserializeAsync<EventGridEvent[]>(context.Request.Body);

            eventGridEvent = eventGridEvents?.FirstOrDefault();
            
            // Validate whether EventType is of "Microsoft.EventGrid.SubscriptionValidationEvent"
            if (eventGridEvent != null && (string.Equals(eventGridEvent.EventType, "Microsoft.EventGrid.SubscriptionValidationEvent", StringComparison.OrdinalIgnoreCase) || (eventGridEvent.EventType != null && eventGridEvent.EventType.IndexOf("Validation", StringComparison.OrdinalIgnoreCase) > 1)))
            {
                var data = eventGridEvent?.Data as JObject;
                var eventData = data?.ToObject<SubscriptionValidationEventData>();
                var responseData = new SubscriptionValidationResponseData
                {
                    ValidationResponse = eventData?.ValidationCode ?? string.Empty
                };

                if (responseData.ValidationResponse != null)
                {
                    return JObject.FromObject(responseData);
                }
            }
            else
            {
                logger.LogInformation("Handling the Custom Event Grid event message");

                var userAccount = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccountDto>(eventGridEvent?.Data.ToString() ?? string.Empty);

                if (userAccount != null)
                {
                    logger.LogInformation($"Creating User Account for Processing Events: {userAccount.Name}-{userAccount.EmailAddress}");
                    CreateUserAccountCommand command = new(userAccount);
                    await _mediator.Send(command, cancellationToken);
                }
                

                //using (StreamReader reader = new StreamReader(context.Request.Body))
                //{
                //    string requestBody = await reader.ReadToEndAsync();

                //    logger.LogInformation("Deserialize the event");

                //    // Deserialize the event
                //    var events = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomEvent<UserAccountDto>[]>(requestBody);

                //    logger.LogInformation($"Processing Events: {events?.Any()}");

                //    if (events != null)
                //    {
                //        var customEvents = events.Select(x => x.Data).ToList();

                //        // Process the events
                //        foreach (var userAccount in customEvents)
                //        {
                //            logger.LogInformation($"Creating User Account for Processing Events: {userAccount.Name}-{userAccount.EmailAddress}");
                //            CreateUserAccountCommand command = new(userAccount);
                //            await _mediator.Send(command, cancellationToken);
                //        }
                //    }
                //}
            }

            var responseDataResult = new SubscriptionValidationResponseData
            {
                ValidationResponse = "Done"
            };
            return JObject.FromObject(responseDataResult);
        });
    }
}
