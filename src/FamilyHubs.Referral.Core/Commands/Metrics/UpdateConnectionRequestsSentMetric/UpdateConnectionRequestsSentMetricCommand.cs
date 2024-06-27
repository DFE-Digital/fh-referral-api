using AutoMapper;
using FamilyHubs.Referral.Core.ClientServices;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities.Metrics;
using FamilyHubs.Referral.Data.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;

//todo: response belongs in referral shared
public class UpdateConnectionRequestsSentMetricResponse
{

}


public class UpdateConnectionRequestsSentMetricCommand : IRequest<UpdateConnectionRequestsSentMetricResponse>, IUpdateConnectionRequestsSentMetricCommand
{
    public UpdateConnectionRequestsSentMetricCommand()
    {
    }
}

public class UpdateConnectionRequestsSentMetricCommandHandler : IRequestHandler<
    UpdateConnectionRequestsSentMetricCommand, UpdateConnectionRequestsSentMetricResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IServiceDirectoryService _serviceDirectoryService;
    private readonly ILogger<CreateReferralCommandHandler> _logger;

    public UpdateConnectionRequestsSentMetricCommandHandler(ApplicationDbContext context, IMapper mapper,
        IServiceDirectoryService serviceDirectoryService, ILogger<CreateReferralCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _serviceDirectoryService = serviceDirectoryService;
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
        metric.HttpResponseCode = (byte)HttpStatusCode.OK;
        metric.ConnectionRequestId = 1;
        //or pass down
        metric.ConnectionRequestReferenceCode = metric.ConnectionRequestId.Value.ToString("X6");

        _context.Update(metric);
        await _context.SaveChangesAsync();
    }
}