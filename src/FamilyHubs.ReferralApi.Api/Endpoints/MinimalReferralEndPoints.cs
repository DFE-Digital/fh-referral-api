using FamilyHubs.ServiceDirectoryCaseManagement.Common.Dto;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FamilyHubs.ReferralApi.Api.Queries.GetReferrals;
using FamilyHubs.ReferralApi.Api.Commands.CreateReferral;

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

        app.MapGet("api/organisationreferrals/{organisationid}", async (string organisationId, int? pageNumber, int? pageSize, CancellationToken cancellationToken, ISender _mediator) =>
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

        app.MapGet("api/referral/{id}", async (string id, CancellationToken cancellationToken, ISender _mediator) =>
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
    }
}
