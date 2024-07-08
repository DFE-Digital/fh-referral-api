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
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;

namespace FamilyHubs.Referral.Api.Endpoints;

public class MinimalReferralEndPoints
{
    public void RegisterReferralEndPoints(WebApplication app)
    {
        app.MapPost("api/referrals",
            [Authorize(Roles = RoleGroups.LaProfessionalOrDualRole)]
            async (
                [FromBody] CreateReferralDto request,
                CancellationToken cancellationToken,
                ISender mediator,
                HttpContext httpContext) =>
            {
                CreateReferralCommand command = new(request, httpContext.GetFamilyHubsUser());
                var result = await mediator.Send(command, cancellationToken);
                return result;

            }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Create Referral")
            { Tags = new[] { "Referrals" } });

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

        app.MapGet("api/referralsByReferrer/{referrerId}", [Authorize(Roles = RoleGroups.LaProfessionalOrDualRole)] async (long referrerId, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralsByReferrerByReferrerIdCommand request = new(referrerId, orderBy, isAssending, includeDeclined, pageNumber, pageSize);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Referrer Id") { Tags = new[] { "Referrals" } });

        app.MapGet("api/organisationreferrals/{organisationId}", [Authorize(Roles = RoleGroups.VcsProfessionalOrDualRole)] async (long organisationId, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralsByOrganisationIdCommand request = new(organisationId, orderBy, isAssending, includeDeclined, pageNumber, pageSize);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Organisation Id") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referral/{id}", [Authorize(Roles = RoleGroups.LaProfessionalOrDualRole+","+RoleGroups.VcsProfessionalOrDualRole+","+ RoleTypes.LaManager)] async (long id, CancellationToken cancellationToken, ISender _mediator, HttpContext httpContext) =>
        {
            //todo: use HttpContext.GetFamilyHubsUser() instead?
            (long accountId, string role, long organisationId) = GetUserDetailsFromClaims(httpContext);
           
            GetReferralByIdCommand request = new(id);
            var result = await _mediator.Send(request, cancellationToken);

            //If this is a VCS User make sure they can only see their own organisation details
            // VcsManagers will be blocked at the endpoint, but the check still makes sense here
            if (role is RoleTypes.VcsManager or RoleTypes.VcsProfessional or RoleTypes.VcsDualRole
                && result.ReferralServiceDto.OrganisationDto.Id != organisationId)
            {
                return await SetForbidden<ReferralDto>(httpContext);
            }

            if (role is RoleTypes.LaProfessional or RoleTypes.LaDualRole
                && accountId != result.ReferralUserAccountDto.Id)
            {
                return await SetForbidden<ReferralDto>(httpContext);
            }

            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referral By Id") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referral/compositekeys", [Authorize(Policy = "ReferralUser")] async (long? serviceId, long? statusId, long? recipientId, long? referralId, ReferralOrderBy? orderBy, bool? isAssending, bool? includeDeclined, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralByServiceIdStatusIdRecipientIdReferrerIdCommand request = new(serviceId, statusId, recipientId, referralId,orderBy, isAssending, includeDeclined, pageNumber, pageSize);
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referral By Composite Keys") { Tags = new[] { "Referrals" } });

        app.MapPost("api/status/{referralId}/{status}", [Authorize(Roles = RoleGroups.VcsProfessionalOrDualRole)] async (long referralId, string status, CancellationToken cancellationToken, ISender _mediator, HttpContext httpContext, ILogger < MinimalReferralEndPoints> logger) =>
        {
            (long _, string role, long organisationId) = GetUserDetailsFromClaims(httpContext);

            SetReferralStatusCommand command = new(role, organisationId, referralId, status, default!);
            var result = await _mediator.Send(command, cancellationToken);
            if (result == SetReferralStatusCommandHandler.Forbidden) 
            {
                return await SetForbidden<string>(httpContext);
            }
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Set Referral Status") { Tags = new[] { "Referrals" } });

        app.MapPost("api/status/{referralId}/{status}/{reasonForDecliningSupport}", [Authorize(Roles = RoleGroups.VcsProfessionalOrDualRole)] async (long referralId, string status, string reasonForDecliningSupport, CancellationToken cancellationToken, ISender _mediator, HttpContext httpContext, ILogger <MinimalReferralEndPoints> logger) =>
        {
            (long _, string role, long organisationId) = GetUserDetailsFromClaims(httpContext);

            SetReferralStatusCommand command = new(role, organisationId, referralId, status, reasonForDecliningSupport);
            var result = await _mediator.Send(command, cancellationToken);
            if (result == SetReferralStatusCommandHandler.Forbidden)
            {
                return await SetForbidden<string>(httpContext);
            }
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Set Referral Status") { Tags = new[] { "Referrals" } });

        app.MapGet("api/statuses", [Authorize(Policy = "ReferralUser")] async (CancellationToken cancellationToken, ISender _mediator) =>
        {
            GetReferralStatusesCommand request = new();
            var result = await _mediator.Send(request, cancellationToken);
            return result;

        }).WithMetadata(new SwaggerOperationAttribute("Get Referral Statuses", "Get Referral Statuses") { Tags = new[] { "Referrals" } });
#pragma warning disable S1481
        app.MapGet("api/referral/recipient", [Authorize(Roles = $"{RoleTypes.LaManager},{RoleTypes.LaProfessional},{RoleTypes.LaDualRole}")] async (string? email, string? telephone, string? textphone, string? name, string? postcode, CancellationToken cancellationToken, ISender _mediator, HttpContext httpContext) =>
        {
            (long accountId, string role, long organisationId) = GetUserDetailsFromClaims(httpContext);

            if (role is RoleTypes.LaManager or RoleTypes.LaProfessional or RoleTypes.LaDualRole)
            {
                GetReferralsByRecipientCommand request = new(organisationId, email, telephone, textphone, name, postcode);
                var result = await _mediator.Send(request, cancellationToken);
                return result;
            }

            return await SetForbidden<List<ReferralDto>>(httpContext);    

        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referral By Recipient") { Tags = new[] { "Referrals" } });
#pragma warning restore S1481
    }

    private async Task<T> SetForbidden<T>(HttpContext httpContext)
    {
        var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());
        var statusCodeResult = new StatusCodeResult(StatusCodes.Status403Forbidden);
        await statusCodeResult.ExecuteResultAsync(actionContext);
        return default!;
    }

    private (long accountId, string role, long organisationId) GetUserDetailsFromClaims(HttpContext httpContext)
    {
        //todo: should really throw if claim is missing, rather than defaulting
        long accountId = -1;
        var accountIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == FamilyHubsClaimTypes.AccountId);
        if (accountIdClaim != null)
        {
            long.TryParse(accountIdClaim.Value, out accountId);
        }

        long organisationId = -1;
        var organisationIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == FamilyHubsClaimTypes.OrganisationId);
        if (organisationIdClaim != null) 
        {
            long.TryParse(organisationIdClaim.Value, out organisationId);
        }

        string role = string.Empty;
        var roleClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (roleClaim != null) 
        {
            role = roleClaim.Value;
        }

        return (accountId, role, organisationId);
    }
}
