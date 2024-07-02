using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Repository;
using MediatR;
using System.Diagnostics;
using FamilyHubs.Referral.Data.Entities.Metrics;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.SharedKernel.Identity.Models;

namespace FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;

public record UpdateConnectionRequestsSentMetricCommand(
    UpdateConnectionRequestsSentMetricDto MetricDto,
    FamilyHubsUser FamilyHubsUser)
    : IRequest<Unit>, IUpdateConnectionRequestsSentMetricCommand
{
}

public class UpdateConnectionRequestsSentMetricCommandHandler : IRequestHandler<
    UpdateConnectionRequestsSentMetricCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateConnectionRequestsSentMetricCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(
        UpdateConnectionRequestsSentMetricCommand request, CancellationToken cancellationToken)
    {
        await WriteCreateReferralUpdateMetrics(request);

        return Unit.Value;
    }

    private async Task WriteCreateReferralUpdateMetrics(UpdateConnectionRequestsSentMetricCommand request)
    {
        string traceId = Activity.Current!.TraceId.ToString();
        var metric = _context.ConnectionRequestsSentMetric.SingleOrDefault(m => m.RequestCorrelationId == traceId);

        if (metric == null)
        {
            // even though we write the metric out in its own transaction, before creating the referral in another transaction,
            // we still might not have an existing metric, if the original create referral call fails before the metric row is created (e.g. a transitory network issue calling the endpoint),
            // but the call to update the metric from the exception handler is ok
            metric = new ConnectionRequestsSentMetric
            {
                RequestCorrelationId = traceId,
                OrganisationId = long.Parse(request.FamilyHubsUser.OrganisationId),
                UserAccountId = long.Parse(request.FamilyHubsUser.AccountId),
                RequestTimestamp = request.MetricDto.RequestTimestamp.DateTime
            };

            _context.Add(metric);
        }
        else
        {
            _context.Update(metric);
        }

        metric.ResponseTimestamp = DateTime.UtcNow;
        metric.HttpResponseCode = request.MetricDto.HttpStatusCode;
        if (request.MetricDto.ConnectionRequestId != null)
        {
            metric.ConnectionRequestId = request.MetricDto.ConnectionRequestId;
            metric.ConnectionRequestReferenceCode = request.MetricDto.ConnectionRequestId.Value.ToString("X6");
        }

        await _context.SaveChangesAsync();
    }
}