namespace FamilyHubs.Referral.Api.Middleware;

public class CorrelationMiddleware : IMiddleware
{
    private readonly ILogger<CorrelationMiddleware> _logger;

    public CorrelationMiddleware(ILogger<CorrelationMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.Request?.Headers["X-Correlation-ID"].ToString();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await next(context);
        }
    }
}

