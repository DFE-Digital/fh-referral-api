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
        app.MapPost("api/referrals", [Authorize(Roles = RoleGroups.LaProfessionalOrDualRole)] async ([FromBody] ReferralDto request, CancellationToken cancellationToken, ISender _mediator) =>
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

        //todo: add new ProfessionalOrDualRole RoleGroup (or LaOrVcsProfessionalOrDualRole)
        app.MapGet("api/referral/{id}", [Authorize(Roles = RoleGroups.LaProfessionalOrDualRole+","+RoleGroups.VcsProfessionalOrDualRole)] async (long id, CancellationToken cancellationToken, ISender _mediator, HttpContext httpContext) =>
        {
            (string email, string role, long organisationId) = GetUserDetailsFromClaims(httpContext);
           
            GetReferralByIdCommand request = new(id);
            var result = await _mediator.Send(request, cancellationToken);

            //If this is a VCS User make sure they can only see their own organisation details
            // VcsManagers will be blocked at the endpoint, but the check still makes sense here
            if (role is RoleTypes.VcsManager or RoleTypes.VcsProfessional or RoleTypes.VcsDualRole
                && result.ReferralServiceDto.OrganisationDto.Id != organisationId)
            {
                return await SetForbidden<ReferralDto>(httpContext);
            }

            if (role is RoleTypes.LaManager or RoleTypes.LaProfessional or RoleTypes.LaDualRole
                && !AreEmailsEqual(email, result.ReferralUserAccountDto.EmailAddress))
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
            (string _, string role, long organisationId) = GetUserDetailsFromClaims(httpContext);

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
            (string _, string role, long organisationId) = GetUserDetailsFromClaims(httpContext);

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
    }

    private async Task<T> SetForbidden<T>(HttpContext httpContext)
    {
        var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());
        var statusCodeResult = new StatusCodeResult(StatusCodes.Status403Forbidden);
        await statusCodeResult.ExecuteResultAsync(actionContext);
        return default!;
    }

    private (string email, string role, long organisationId) GetUserDetailsFromClaims(HttpContext httpContext)
    {
        string email = string.Empty;
        var roleEmail = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        if (roleEmail != null)
        {
            email = roleEmail.Value;
        }

        long organisationId = 0;
        var organisationIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "OrganisationId");
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

        return (email, role, organisationId);
    }

    // the email will probably always be the same case as we get them from the claim, but just to be extra safe
    public static bool AreEmailsEqual(string email1, string email2)
    {
        int atIndex1 = email1.IndexOf('@');
        int atIndex2 = email2.IndexOf('@');

        if (atIndex1 == -1 || atIndex2 == -1)
        {
            // Both email strings should have exactly one '@' symbol
            return false;
        }

        string localPart1 = email1.Substring(0, atIndex1);
        string localPart2 = email2.Substring(0, atIndex2);
        string domainPart1 = email1.Substring(atIndex1 + 1);
        string domainPart2 = email2.Substring(atIndex2 + 1);

        bool areLocalPartsEqual = string.Equals(localPart1, localPart2, StringComparison.Ordinal);
        bool areDomainPartsEqual = string.Equals(domainPart1, domainPart2, StringComparison.OrdinalIgnoreCase);

        return areLocalPartsEqual && areDomainPartsEqual;
    }
}
