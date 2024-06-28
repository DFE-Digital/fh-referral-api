using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using System.Diagnostics;

namespace FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;

//todo: validation?

public record UpdateConnectionRequestsSentMetricCommand(UpdateConnectionRequestsSentMetricDto MetricDto)
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
        //todo: index on RequestCorrelationId?
        var metric = _context.ConnectionRequestsSentMetric.Single(m => m.RequestCorrelationId == traceId);

        metric.ResponseTimestamp = DateTime.UtcNow;
        metric.HttpResponseCode = request.MetricDto.HttpStatusCode;
        if (request.MetricDto.ConnectionRequestId != null)
        {
            metric.ConnectionRequestId = request.MetricDto.ConnectionRequestId;
            metric.ConnectionRequestReferenceCode = request.MetricDto.ConnectionRequestId.ToString("X6");
        }

        _context.Update(metric);
        await _context.SaveChangesAsync();
    }
}