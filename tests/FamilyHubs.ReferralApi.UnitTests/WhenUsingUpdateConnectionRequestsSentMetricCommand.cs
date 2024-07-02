using FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;
using FamilyHubs.Referral.Data.Entities.Metrics;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.SharedKernel.Identity.Models;
using System.Net;
using FluentAssertions;

namespace FamilyHubs.Referral.UnitTests;

public class WhenUsingUpdateConnectionRequestsSentMetricCommand : BaseCreateDbUnitTest
{
    public DateTimeOffset RequestTimestamp { get; set; }
    public FamilyHubsUser FamilyHubsUser { get; set; }
    public const long ExpectedAccountId = 123L;
    public const long ExpectedOrganisationId = 456L;

    public WhenUsingUpdateConnectionRequestsSentMetricCommand()
    {
        RequestTimestamp = new DateTimeOffset(new DateTime(2025, 1, 1, 12, 0, 0));

        FamilyHubsUser = new FamilyHubsUser
        {
            AccountId = ExpectedAccountId.ToString(),
            OrganisationId = ExpectedOrganisationId.ToString()
        };
    }

    [Fact]
    public async Task ThenMetricIsUpdatedWhenItAlreadyExistsAndCreateReferralSucceeded()
    {
        var mockApplicationDbContext = GetApplicationDbContext();
        mockApplicationDbContext.ConnectionRequestsSentMetric.Add(new ConnectionRequestsSentMetric
        {
            OrganisationId = ExpectedOrganisationId,
            UserAccountId = ExpectedAccountId,
            RequestTimestamp = RequestTimestamp.DateTime,
            RequestCorrelationId = ExpectedRequestCorrelationId
        });
        await mockApplicationDbContext.SaveChangesAsync();

        const HttpStatusCode httpStatusCode = HttpStatusCode.NoContent;
        const long connectionRequestId = 17L;
        const string connectionRequestReferenceCode = "000011";

        // the request timestamp passed to update should be the same that was passed to create referral,
        // but we pass a different timestamp, so that we can check that the original create referral timestamp isn't updated
        DateTimeOffset updateRequestTimestamp = new DateTimeOffset(new DateTime(2026, 2, 2, 11, 11, 0));

        UpdateConnectionRequestsSentMetricDto metric = new(updateRequestTimestamp, httpStatusCode, connectionRequestId);
        UpdateConnectionRequestsSentMetricCommand command = new(metric, FamilyHubsUser);
        UpdateConnectionRequestsSentMetricCommandHandler handler = new(mockApplicationDbContext);

        //Act
        var result = await handler.Handle(command, new CancellationToken());

        mockApplicationDbContext.ConnectionRequestsSentMetric.Should().NotBeEmpty();
        var actualMetric = mockApplicationDbContext.ConnectionRequestsSentMetric.SingleOrDefault();
        actualMetric.Should().NotBeNull();
        actualMetric!.OrganisationId.Should().Be(ExpectedOrganisationId);
        actualMetric.UserAccountId.Should().Be(ExpectedAccountId);
        actualMetric.RequestTimestamp.Should().Be(RequestTimestamp.DateTime);
        actualMetric.RequestCorrelationId.Should().Be(ExpectedRequestCorrelationId);
        //todo: hacky, change to TimeProvider when we upgrade to .net 8
        actualMetric.ResponseTimestamp.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0, 1, 2));
        actualMetric.HttpResponseCode.Should().Be(httpStatusCode);
        actualMetric.ConnectionRequestId.Should().Be(connectionRequestId);
        actualMetric.ConnectionRequestReferenceCode.Should().Be(connectionRequestReferenceCode);
    }

    [Fact]
    public async Task ThenMetricIsCreatedWhenItDoesNotAlreadyExists()
    {
        var mockApplicationDbContext = GetApplicationDbContext();

        const HttpStatusCode httpStatusCode = HttpStatusCode.BadGateway;
        long? connectionRequestId = null;
        const string? connectionRequestReferenceCode = null;

        var command = new UpdateConnectionRequestsSentMetricCommand(
            new UpdateConnectionRequestsSentMetricDto(RequestTimestamp, httpStatusCode, connectionRequestId),
            FamilyHubsUser);

        UpdateConnectionRequestsSentMetricCommandHandler handler = new(mockApplicationDbContext);

        //Act
        await handler.Handle(command, new CancellationToken());

        mockApplicationDbContext.ConnectionRequestsSentMetric.Should().NotBeEmpty();
        var actualMetric = mockApplicationDbContext.ConnectionRequestsSentMetric.SingleOrDefault();
        actualMetric.Should().NotBeNull();
        actualMetric!.OrganisationId.Should().Be(ExpectedOrganisationId);
        actualMetric.UserAccountId.Should().Be(ExpectedAccountId);
        actualMetric.RequestTimestamp.Should().Be(RequestTimestamp.DateTime);
        actualMetric.RequestCorrelationId.Should().Be(ExpectedRequestCorrelationId);
        //todo: hacky, change to TimeProvider when we upgrade to .net 8
        actualMetric.ResponseTimestamp.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0, 1, 2));
        actualMetric.HttpResponseCode.Should().Be(httpStatusCode);
        actualMetric.ConnectionRequestId.Should().Be(connectionRequestId);
        actualMetric.ConnectionRequestReferenceCode.Should().Be(connectionRequestReferenceCode);
    }
}