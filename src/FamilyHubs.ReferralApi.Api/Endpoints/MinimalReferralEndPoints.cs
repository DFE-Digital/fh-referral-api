using FamilyHubs.ReferralApi.Core.Commands.CreateReferral;
using FamilyHubs.ReferralApi.Core.Commands.SetReferralStatus;
using FamilyHubs.ReferralApi.Core.Commands.UpdateReferral;
using FamilyHubs.ReferralApi.Core.Queries.GetReferrals;
using FamilyHubs.ServiceDirectory.Shared.Dto.Referral;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FamilyHubs.ReferralApi.Api.Endpoints;

public class MinimalReferralEndPoints
{
    public void RegisterReferralEndPoints(WebApplication app)
    {
        app.MapPost("api/referrals", async ([FromBody] ReferralDto request, CancellationToken cancellationToken, ISender _mediator) =>
        {
            try
            {
                CreateReferralCommand command = new(request);
                var result = await _mediator.Send(command, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Create Referral") { Tags = new[] { "Referrals" } });

        app.MapPut("api/referrals/{id}", async (long id, [FromBody] ReferralDto request, CancellationToken cancellationToken, ISender _mediator, ILogger<MinimalReferralEndPoints> logger) =>
        {
            try
            {
                UpdateReferralCommand command = new(id, request);
                var result = await _mediator.Send(command, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred updating referral (api). {exceptionMessage}", ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }).WithMetadata(new SwaggerOperationAttribute("Update Referral", "Update Referral By Id") { Tags = new[] { "Referrals" } });


        app.MapGet("api/referrals/{referrer}", async (string referrer, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            try
            {
                GetReferralsByReferrerCommand request = new(referrer, pageNumber, pageSize);
                var result = await _mediator.Send(request, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Referrer") { Tags = new[] { "Referrals" } });

        app.MapGet("api/organisationreferrals/{organisationId}", async (long organisationId, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
        {
            try
            {
                GetReferralsByOrganisationIdCommand request = new(organisationId, pageNumber, pageSize);
                var result = await _mediator.Send(request, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referrals By Organisation Id") { Tags = new[] { "Referrals" } });

        app.MapGet("api/referral/{id}", async (long id, CancellationToken cancellationToken, ISender _mediator) =>
        {
            try
            {
                GetReferralByIdCommand request = new(id);
                var result = await _mediator.Send(request, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }).WithMetadata(new SwaggerOperationAttribute("Get Referrals", "Get Referral By Id") { Tags = new[] { "Referrals" } });

        app.MapPost("api/referralStatus/{referralId}/{status}", async (long referralId, string status, CancellationToken cancellationToken, ISender _mediator, ILogger<MinimalReferralEndPoints> logger) =>
        {
            try
            {
                SetReferralStatusCommand command = new(referralId, status);
                var result = await _mediator.Send(command, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred setting referral status (api). {exceptionMessage}", ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }).WithMetadata(new SwaggerOperationAttribute("Referrals", "Set Referral Status") { Tags = new[] { "Referrals" } });
    }
}
