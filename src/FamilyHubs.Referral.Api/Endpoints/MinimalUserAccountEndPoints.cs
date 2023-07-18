using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Core.Queries.GetUserAccounts;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Swashbuckle.AspNetCore.Annotations;

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

            if (context.Request.Headers.TryGetValue("aeg-event-type", out var eventType) && eventType == "SubscriptionValidation")
            {
                logger.LogInformation("Handling Event Grid validation message");

                // Deserialize the validation event
                var eventData = await System.Text.Json.JsonSerializer.DeserializeAsync<SubscriptionValidationEventData>(context.Request.Body);
               
                // Send back the validation response
                var responseData = new SubscriptionValidationResponseData { ValidationResponse = eventData?.ValidationCode ?? string.Empty };
                var responseJson = System.Text.Json.JsonSerializer.Serialize(responseData);
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(responseJson);
            }
            else
            {
                logger.LogInformation("Handling Event Grid event message");

                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                    string requestBody = await reader.ReadToEndAsync();

                    logger.LogInformation("Deserialize the event");

                    // Deserialize the event
                    var events = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomEvent<UserAccountDto>[]>(requestBody);

                    logger.LogInformation($"Processing Events: {events?.Any()}");

                    if (events != null)
                    {
                        var customEvents = events.Select(x => x.Data).ToList();

                        // Process the events
                        foreach (var userAccount in customEvents)
                        {
                            logger.LogInformation($"Creating User Account for Processing Events: {userAccount.Name}-{userAccount.EmailAddress}");
                            CreateUserAccountCommand command = new(userAccount);
                            await _mediator.Send(command, cancellationToken);
                        }
                    }
                }
            }
        });
    }
}
