using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.SetReferralStatus;
using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FamilyHubs.Referral.Core.Queries.GetReferralStatus;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FamilyHubs.Referral.Api.Endpoints;

public class MinimalReferralEndPoints
{
    public void RegisterReferralEndPoints(WebApplication app)
    {
        app.MapPost("api/referrals", [Authorize] async ([FromBody] ReferralDto request, CancellationToken cancellationToken, ISender _mediator) =>
        {
            CreateReferralCommand command = new(request);
            var result = await _mediator.Send(command, cancellationToken);
            return result;
            
        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Create Referral") { Tags = new[] { "Referrals" } });

        app.MapPut("api/referrals/{id}", [Authorize] async (long id, [FromBody] ReferralDto request, CancellationToken cancellationToken, ISender _mediator, ILogger<MinimalReferralEndPoints> logger) =>
        {
            UpdateReferralCommand command = new(id, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Update Referral", "Update Referral By Id") { Tags = new[] { "Referrals" } });


        app.MapGet("api/referrals/{referrer}", [Authorize] async (string referrer, int? pageNumber, int? pageSize, string? searchText, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralsByReferrerCommand request = new(referrer, pageNumber, pageSize, searchText);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Referrer") { Tags = new[] { "Referrals" } });

        app.MapGet("api/organisationreferrals/{organisationId}", [Authorize] async (long organisationId, int? pageNumber, int? pageSize, string ? searchText, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralsByOrganisationIdCommand request = new(organisationId, pageNumber, pageSize, searchText);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Organisation Id") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referral/{id}", [Authorize] async (long id, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralByIdCommand request = new(id);
            var result = await _mediator.Send(request, cancellationToken);
            return result;
            
        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referral By Id") { Tags = new[] { "Referrals" } });

        app.MapPost("api/referralStatus/{referralId}/{status}", [Authorize] async (long referralId, string status, CancellationToken cancellationToken, ISender _mediator, ILogger<MinimalReferralEndPoints> logger) =>
        {
            SetReferralStatusCommand command = new(referralId, status);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Set Referral Status") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referralstatuses", [Authorize] async (CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralStatusesCommand request = new();
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referral Statuses", "Get Referral Statuses") { Tags = new[] { "Referrals" } });
    }
}
