using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Repository;
using MediatR;
using System.Diagnostics;

namespace FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;

//todo: response belongs in referral shared
public class UpdateConnectionRequestsSentMetricResponse
{
}

public record UpdateConnectionRequestsSentMetricCommand(byte HttpResponseCode, long ConnectionRequestId)
    : IRequest<UpdateConnectionRequestsSentMetricResponse>, IUpdateConnectionRequestsSentMetricCommand
{
}

public class UpdateConnectionRequestsSentMetricCommandHandler : IRequestHandler<
    UpdateConnectionRequestsSentMetricCommand, UpdateConnectionRequestsSentMetricResponse>
{
    private readonly ApplicationDbContext _context;

    public UpdateConnectionRequestsSentMetricCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateConnectionRequestsSentMetricResponse> Handle(
        UpdateConnectionRequestsSentMetricCommand request, CancellationToken cancellationToken)
    {
        await WriteCreateReferralUpdateMetrics(request);

        return new UpdateConnectionRequestsSentMetricResponse();
    }

    private async Task WriteCreateReferralUpdateMetrics(UpdateConnectionRequestsSentMetricCommand request)
    {
        string traceId = Activity.Current!.TraceId.ToString();
        //todo: index on RequestCorrelationId?
        var metric = _context.ConnectionRequestsSentMetric.Single(m => m.RequestCorrelationId == traceId);

        metric.ResponseTimestamp = DateTime.UtcNow;
        metric.HttpResponseCode = request.HttpResponseCode;
        metric.ConnectionRequestId = request.ConnectionRequestId;
        metric.ConnectionRequestReferenceCode = request.ConnectionRequestId.ToString("X6");

        _context.Update(metric);
        await _context.SaveChangesAsync();
    }
}