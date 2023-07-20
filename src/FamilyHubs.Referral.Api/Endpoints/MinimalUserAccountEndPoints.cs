using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Messaging.EventGrid;
using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Core.Queries.GetUserAccounts;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json;
using FamilyHubs.Referral.Data.Models;
using Newtonsoft.Json;

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

            var syncIOFeature = context.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            EventGridEventEx[] egEvents = default!;

            using JsonDocument requestDocument = JsonDocument.Parse(context.Request.Body);
            if (requestDocument.RootElement.ValueKind == JsonValueKind.Object)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<EventGridEventEx>(requestDocument.RootElement.ToString());
                if (item != null)
                {
                    //Type? myType = Type.GetType(item.EventType);
                    //if (myType != null && item.Data != null)
                    //{
                    //    string data = item.Data.ToString() ?? string.Empty;
                    //    if (data.StartsWith("{{"))
                    //    {
                    //        data = data.Substring(1, data.Length - 2) ?? string.Empty;
                    //    }
                    //    item.Data = JsonConvert.DeserializeObject(data, myType);
                    //}

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
                        
                        //Type? myType = Type.GetType(item.EventType);
                        //if (myType != null && item.Data != null)
                        //{
                        //    string data = item.Data.ToString() ?? string.Empty;
                        //    if (data.StartsWith("{{"))
                        //    {
                        //        data = data.Substring(1, data.Length - 2) ?? string.Empty;
                        //    }
                        //    item.Data = JsonConvert.DeserializeObject(data, myType);
                        //}
                        egEvents[i++] = item;
                    }
                    
                }
            }

            if (egEvents == null)
            {
                var badresponseDataResult = new SubscriptionValidationResponseData
                {
                    ValidationResponse = "Event Not Handled"
                };
                return JObject.FromObject(badresponseDataResult);
            }

            foreach (EventGridEventEx egEvent in egEvents)
            {
                if (egEvent.Data == null)
                {
                    var badresponseDataResult = new SubscriptionValidationResponseData
                    {
                        ValidationResponse = "Event Not Handled"
                    };
                    return JObject.FromObject(badresponseDataResult);

                }
                if (egEvent.EventType.IndexOf("Validation", StringComparison.OrdinalIgnoreCase) > 1)
                {
                    dynamic item = egEvent.Data;
                    var responseData = new SubscriptionValidationResponseData
                    {
                        ValidationResponse = item.ValidationCode
                    };
                    logger.LogInformation($"ValidationCode: {item.ValidationCode}");
                    return JObject.FromObject(responseData);
                }
                else
                {
                    //Process Custom Event
                    logger.LogInformation("Handling the Custom Event Grid event message");

                    var userAccount = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccountDto>(egEvent.Data.ToString() ?? string.Empty);

                    if (userAccount != null)
                    {
                        logger.LogInformation($"Creating User Account for Processing Events: {userAccount.Name}-{userAccount.EmailAddress}");

                        try
                        {
                            GetUserByIdCommand getUserByIdCommand = new(userAccount.Id);
                            var result = await _mediator.Send(getUserByIdCommand, cancellationToken);
                            if (result != null)
                            {
                                UpdateUserAccountCommand updateUserAccountCommand = new UpdateUserAccountCommand(result.Id, userAccount);
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
    
            var responseDataResult = new SubscriptionValidationResponseData
            {
                ValidationResponse = "Done"
            };
            return JObject.FromObject(responseDataResult);

        });
    }
}
