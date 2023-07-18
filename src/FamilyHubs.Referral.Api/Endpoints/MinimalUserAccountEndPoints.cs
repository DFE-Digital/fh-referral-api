using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Core.Queries.GetUserAccounts;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
namespace FamilyHubs.Referral.Api.Endpoints;

//generic wrapper for Azure EventGrid custom event
public class CustomEvent<T>
{

    public string Id { get; private set; }

    public string EventType { get; set; } = default!;

    public string Subject { get; set; } = default!;

    public string EventTime { get; private set; }

    public T Data { get; set; } = default!;

    public CustomEvent()
    {
        Id = Guid.NewGuid().ToString();

        DateTime localTime = DateTime.Now;
        DateTimeOffset localTimeAndOffset =
            new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
        EventTime = localTimeAndOffset.ToString("o");
    }
}

public class MinimalUserAccountEndPoints
{
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

            using (StreamReader reader = new StreamReader(context.Request.Body))
            {
                string requestBody = await reader.ReadToEndAsync();

                logger.LogInformation("Deserialize the event");

                // Deserialize the event
                var events = JsonConvert.DeserializeObject<CustomEvent<UserAccountDto>[]>(requestBody);

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
        });
    }
}
