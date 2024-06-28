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
        app.MapPost("api/metrics/connection-request", [Authorize(Roles = RoleGroups.LaProfessionalOrDualRole)]
            async ([FromBody] UpdateConnectionRequestsSentMetricDto request,
                CancellationToken cancellationToken,
                ISender mediator,
                HttpContext httpContext) =>
            {
                UpdateConnectionRequestsSentMetricCommand command = new(request);
                await mediator.Send(command, cancellationToken);
                return Results.Ok();

            }).WithMetadata(new SwaggerOperationAttribute("Metrics", "Update Connection Requests Sent Metric")
            { Tags = new[] { "Metrics" } });
    }
}