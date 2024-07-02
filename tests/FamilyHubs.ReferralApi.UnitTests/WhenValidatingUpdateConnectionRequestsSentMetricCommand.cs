using System.Net;
using FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.SharedKernel.Identity.Models;
using FluentAssertions;

namespace FamilyHubs.Referral.UnitTests;

public class WhenValidatingUpdateConnectionRequestsSentMetricCommand
{
    public DateTimeOffset RequestTimestamp { get; set; }
    public FamilyHubsUser FamilyHubsUser { get; set; }
    public const long ExpectedAccountId = 123L;
    public const long ExpectedOrganisationId = 456L;

    public WhenValidatingUpdateConnectionRequestsSentMetricCommand()
    {
        RequestTimestamp = new DateTimeOffset(new DateTime(2025, 1, 1, 12, 0, 0));

        FamilyHubsUser = new FamilyHubsUser
        {
            AccountId = ExpectedAccountId.ToString(),
            OrganisationId = ExpectedOrganisationId.ToString()
        };
    }

    [Fact]
    public void ThenShouldNotErrorWhenModelIsValidAndRepresentsSuccessfulReferralCreation()
    {
        var dto = new UpdateConnectionRequestsSentMetricDto(RequestTimestamp, HttpStatusCode.NoContent, 1L);

        var validator = new UpdateConnectionRequestsSentMetricCommandValidator();
        var testModel = new UpdateConnectionRequestsSentMetricCommand(dto, FamilyHubsUser);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldNotErrorWhenModelIsValidAndRepresentsUnsuccessfulReferralCreation()
    {
        var dto = new UpdateConnectionRequestsSentMetricDto(RequestTimestamp, HttpStatusCode.InternalServerError, null);

        var validator = new UpdateConnectionRequestsSentMetricCommandValidator();
        var testModel = new UpdateConnectionRequestsSentMetricCommand(dto, FamilyHubsUser);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldNotErrorWhenModelIsValidAndRepresentsUnsuccessfulReferralCreationWithNoStatusCode()
    {
        var dto = new UpdateConnectionRequestsSentMetricDto(RequestTimestamp, null, null);

        var validator = new UpdateConnectionRequestsSentMetricCommandValidator();
        var testModel = new UpdateConnectionRequestsSentMetricCommand(dto, FamilyHubsUser);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldNotErrorWhenModelIsValidAndRepresentsUnsuccessfulReferralCreationWithUnknownStatusCode()
    {
        var dto = new UpdateConnectionRequestsSentMetricDto(RequestTimestamp, (HttpStatusCode)666, null);

        var validator = new UpdateConnectionRequestsSentMetricCommandValidator();
        var testModel = new UpdateConnectionRequestsSentMetricCommand(dto, FamilyHubsUser);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }
}