﻿using System.Diagnostics;
using System.Net;
using FamilyHubs.Referral.Core.Commands.Metrics.UpdateConnectionRequestsSentMetric;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.SharedKernel.Identity.Models;
using FluentAssertions;

namespace FamilyHubs.Referral.Integration.Tests;

public class WhenUsingUpdateConnectionRequestsSentMetricCommand : DataIntegrationTestBase
{
    public DateTimeOffset RequestTimestamp { get; set; }
    public FamilyHubsUser FamilyHubsUser { get; set; }
    public const long ExpectedAccountId = 123L;
    public const long ExpectedOrganisationId = 456L;
    public string ExpectedRequestCorrelationId { get; set; }

    public UpdateConnectionRequestsSentMetricCommand UpdateConnectionRequestsSentMetricCommand { get; set; }

    public WhenUsingUpdateConnectionRequestsSentMetricCommand()
    {
        RequestTimestamp = new DateTimeOffset(new DateTime(2025, 1, 1, 12, 0, 0));

        FamilyHubsUser = new FamilyHubsUser
        {
            AccountId = ExpectedAccountId.ToString(),
            OrganisationId = ExpectedOrganisationId.ToString()
        };

        var activity = new Activity("TestActivity");
        activity.SetParentId(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom());
        activity.Start();
        Activity.Current = activity;
        ExpectedRequestCorrelationId = Activity.Current!.TraceId.ToString();
    }

    [Fact]
    public async Task ThenMetricIsUpdatedWhenItAlreadyExists()
    {
        const HttpStatusCode httpStatusCode = HttpStatusCode.NoContent;
        const long connectionRequestId = 17L;
        const string connectionRequestReferenceCode = "000011";

        UpdateConnectionRequestsSentMetricCommand = new UpdateConnectionRequestsSentMetricCommand(
            new UpdateConnectionRequestsSentMetricDto(RequestTimestamp, httpStatusCode, connectionRequestId),
            FamilyHubsUser);

        UpdateConnectionRequestsSentMetricCommandHandler handler = new(TestDbContext);

        //Act
        await handler.Handle(UpdateConnectionRequestsSentMetricCommand, new CancellationToken());

        TestDbContext.ConnectionRequestsSentMetric.Should().NotBeEmpty();
        var metric = TestDbContext.ConnectionRequestsSentMetric.SingleOrDefault();
        metric.Should().NotBeNull();
        metric!.OrganisationId.Should().Be(ExpectedOrganisationId);
        metric.UserAccountId.Should().Be(ExpectedAccountId);
        metric.RequestTimestamp.Should().Be(RequestTimestamp.DateTime);
        metric.RequestCorrelationId.Should().Be(ExpectedRequestCorrelationId);
        //todo: hacky, change to TimeProvider when we upgrade to .net 8
        metric.ResponseTimestamp.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0, 1, 2));
        metric.HttpResponseCode.Should().Be(httpStatusCode);
        metric.ConnectionRequestId.Should().Be(connectionRequestId);
        metric.ConnectionRequestReferenceCode.Should().Be(connectionRequestReferenceCode);
    }
}
