using FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FamilyHubs.Referral.Api.Endpoints;

public class MetricEndpoints
{
    public void RegisterReferralEndPoints(WebApplication app)
    {
        //todo: if create referral fails (and throws an exception in the client in the ui)
        // we won't have an existing row to update, so we need to upsert and handle not null fields

        app.MapPut("api/metrics/connection-request", [Authorize(Roles = RoleGroups.LaProfessionalOrDualRole)]
            async ([FromBody] UpdateConnectionRequestsSentMetricDto request,
                CancellationToken cancellationToken,
                ISender mediator,
                HttpContext httpContext) =>
            {
                UpdateConnectionRequestsSentMetricCommand command = new(request);
                await mediator.Send(command, cancellationToken);
                return Results.NoContent();

            }).WithMetadata(new SwaggerOperationAttribute("Metrics", "Update Connection Requests Sent Metric")
            { Tags = new[] { "Metrics" } });
    }
}