using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.SetReferralStatus;
using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FamilyHubs.Referral.Core.Queries.GetReferralStatus;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Enums;
using FamilyHubs.SharedKernel.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace FamilyHubs.Referral.Api.Endpoints;

public class MinimalReferralEndPoints
{
    public void RegisterReferralEndPoints(WebApplication app)
    {
        app.MapPost("api/referrals", [Authorize(Policy = "ReferralUser")] async ([FromBody] ReferralDto request, CancellationToken cancellationToken, ISender _mediator) =>
        {
            CreateReferralCommand command = new(request);
            var result = await _mediator.Send(command, cancellationToken);
            return result;
            
        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Create Referral") { Tags = new[] { "Referrals" } });

        app.MapPut("api/referrals/{id}", [Authorize(Policy = "ReferralUser")] async (long id, [FromBody] ReferralDto request, CancellationToken cancellationToken, ISender _mediator, ILogger<MinimalReferralEndPoints> logger) =>
        {
            UpdateReferralCommand command = new(id, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Update Referral", "Update Referral By Id") { Tags = new[] { "Referrals" } });


        app.MapGet("api/referrals/{referrer}", [Authorize(Policy = "ReferralUser")] async (string referrer, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralsByReferrerCommand request = new(referrer, orderBy, isAssending, includeDeclined, pageNumber, pageSize);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Referrer") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referralsByReferrer/{referrerId}", [Authorize(Policy = "ReferralUser")] async (long referrerId, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralsByReferrerByReferrerIdCommand request = new(referrerId, orderBy, isAssending, includeDeclined, pageNumber, pageSize);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Referrer Id") { Tags = new[] { "Referrals" } });

        app.MapGet("api/organisationreferrals/{organisationId}", [Authorize(Policy = "ReferralUser")] async (long organisationId, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralsByOrganisationIdCommand request = new(organisationId, orderBy, isAssending, includeDeclined, pageNumber, pageSize);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Organisation Id") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referral/{id}", [Authorize(Policy = "ReferralUser")] async (long id, CancellationToken cancellationToken, ISender _mediator, HttpContext httpContext) =>
        {
            (string role, long organisationId) = GetUserRoleAndOrgansationFromClaims(httpContext);
           
            GetReferralByIdCommand request = new(id);
            var result = await _mediator.Send(request, cancellationToken);

            //If this is a VCS User make sure they can only see their own organisation details
            if ((role == RoleTypes.VcsManager || role == RoleTypes.VcsProfessional || role == RoleTypes.VcsDualRole) && result.ReferralServiceDto.ReferralOrganisationDto.Id != organisationId)
            {
                var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());
                var statusCodeResult = new StatusCodeResult(StatusCodes.Status403Forbidden);
                await statusCodeResult.ExecuteResultAsync(actionContext);
                return default!;
            }

            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referral By Id") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referral/compositekeys", [Authorize(Policy = "ReferralUser")] async (long? serviceId, long? statusId, long? recipientId, long? referralId, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand request = new(serviceId, statusId, recipientId, referralId,orderBy, isAssending, includeDeclined, pageNumber, pageSize);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referral By Composite Keys") { Tags = new[] { "Referrals" } });

        app.MapPost("api/referralStatus/{referralId}/{status}", [Authorize(Policy = "ReferralUser")] async (long referralId, string status, CancellationToken cancellationToken, ISender _mediator, ILogger<MinimalReferralEndPoints> logger) =>
        {
            SetReferralStatusCommand command = new(referralId, status, default!);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Set Referral Status") { Tags = new[] { "Referrals" } });

        app.MapPost("api/referralStatus/{referralId}/{status}/{reasonForDecliningSupport}", [Authorize(Policy = "ReferralUser")] async (long referralId, string status, string reasonForDecliningSupport, CancellationToken cancellationToken, ISender _mediator, ILogger<MinimalReferralEndPoints> logger) =>
        {
            SetReferralStatusCommand command = new(referralId, status, reasonForDecliningSupport);
            var result = await _mediator.Send(command, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Set Referral Status") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referralstatuses", [Authorize(Policy = "ReferralUser")] async (CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralStatusesCommand request = new();
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referral Statuses", "Get Referral Statuses") { Tags = new[] { "Referrals" } });
    }

    private (string role, long organisationId) GetUserRoleAndOrgansationFromClaims(HttpContext httpContext)
    {
        string role = string.Empty;
        long organisationId = 0;
        var organisationIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "OrganisationId");
        if (organisationIdClaim != null) 
        {
            long.TryParse(organisationIdClaim.Value, out organisationId);
        }

        var roleClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (roleClaim != null) 
        {
            role = roleClaim.Value;
        }

        return (role, organisationId);
    }

}
