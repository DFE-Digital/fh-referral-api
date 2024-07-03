using System.Net;
using FluentValidation;

namespace FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;

public class UpdateConnectionRequestsSentMetricCommandValidator : AbstractValidator<UpdateConnectionRequestsSentMetricCommand>
{
    public UpdateConnectionRequestsSentMetricCommandValidator()
    {
        RuleFor(v => v.MetricDto)
            .NotNull();

        RuleFor(v => v.MetricDto.HttpStatusCode)
            .Must(httpStatusCode => Enum.IsDefined(typeof(HttpStatusCode), httpStatusCode))
            .WithMessage("Invalid HttpStatusCode value.")
            .When(v => v.MetricDto.HttpStatusCode != null);
    }
}
