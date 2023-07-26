﻿using AutoMapper;
using Azure.Messaging.EventGrid.SystemEvents;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.Commands;
using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.Referral.UnitTests;

public class WhenUsingProcessUserGidEventCommands : BaseCreateDbUnitTest
{
    [Fact]
    public async Task ThenHandle_WithInvalidEventData_ReturnsValidationResponse()
    {
        // Arrange
        var handler = CreateHandler();
        var command = CreateCommand(CreateHttpContext("InvalidEventGridEventData"));

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("'I' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0.");
        
    }

    [Fact]
    public async Task ThenHandle_WithSubscriptionValidationEvent_ReturnsValidationResponse()
    {
        // Arrange
        var handler = CreateHandler();
        var eventGridEvent = new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    EventType = typeof(SubscriptionValidationEventData).AssemblyQualifiedName,
                    Subject = "Unit Test",
                    EventTime = DateTime.UtcNow,
                    Data = new
                    {
                        validationCode = "123456"
                    }
                }
            };
        
        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(eventGridEvent);
        var command = CreateCommand(CreateHttpContext(requestBody));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.validationResponse.Should().Be(eventGridEvent[0].Data.validationCode);
    }

    [Fact]
    public async Task ThenHandle_WithSingleSubscriptionValidationEvent_ReturnsValidationResponse()
    {
        // Arrange
        var handler = CreateHandler();
        var eventGridEvent = new
        {
            Id = Guid.NewGuid(),
            EventType = typeof(SubscriptionValidationEventData).AssemblyQualifiedName,
            Subject = "Unit Test",
            EventTime = DateTime.UtcNow,
            Data = new
            {
                validationCode = "123456"
            }
        };

        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(eventGridEvent);
        var command = CreateCommand(CreateHttpContext(requestBody));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.validationResponse.Should().Be(eventGridEvent.Data.validationCode);
    }

    [Fact]
    public async Task ThenHandle_WithCustomEventDataAsUser_CallsProcessUserAccount()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var contextMock = GetApplicationDbContext();
        var handler = CreateHandler(contextMock, mediatorMock.Object);

        // Set up the user account DTO for a regular event message
        UserAccountDto userAccountDto = new UserAccountDto
        {
            Id = 3,
            EmailAddress = "UnitTest@example.com",
            Name = "Unit Test User",
            PhoneNumber = "123456788",
            Team = "Test Team"
        };

        userAccountDto.OrganisationUserAccounts = new List<UserAccountOrganisationDto>
        {
            new UserAccountOrganisationDto
            {
                UserAccount = default!,
                Organisation = new OrganisationDto
                {
                    Id = 2,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }
        };

        var eventData = new[]
        {
            new
            {
                Id = Guid.NewGuid(),
                EventType = "UserAccountDto",
                Subject = "Unit Test",
                EventTime = DateTime.UtcNow,
                Data = userAccountDto
            }
        };
        
        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(eventData);
        var command = CreateCommand(CreateHttpContext(requestBody));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        // Verify that the ProcessUserAccount method was called with the correct parameters
        mediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateUserAccountCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ThenHandle_WithCustomEventDataAsOrganisation_CallsProcessUserAccount()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var contextMock = GetApplicationDbContext();
        var handler = CreateHandler(contextMock, mediatorMock.Object);

        // Set up the user account DTO for a regular event message
        var eventData = new[]
        {
            new
            {
                Id = Guid.NewGuid(),
                EventType = "OrganisationDto",
                Subject = "Unit Test",
                EventTime = DateTime.UtcNow,
                Data = new OrganisationDto
                {
                    Id = 3,
                    Name = "Event Grid Organisation",
                    Description = "Event Grid Organisation Description",
                }
            }
        };

        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(eventData);
        var command = CreateCommand(CreateHttpContext(requestBody));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        

        // Assert
        result.Should().NotBeNull();
        contextMock.Organisations.FirstOrDefault(x => x.Id == eventData[0].Data.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task ThenHandle_WithCustomEvent_WithExistingUser_CallsProcessUserAccount()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        var contextMock = GetApplicationDbContext();
        var handler = CreateHandler(contextMock, mediatorMock.Object);

        // Set up the user account DTO for a regular event message
        UserAccountDto userAccountDto = new UserAccountDto
        {
            Id = 3,
            EmailAddress = "UnitTest@example.com",
            Name = "Unit Test User",
            PhoneNumber = "123456788",
            Team = "Test Team"
        };

        userAccountDto.OrganisationUserAccounts = new List<UserAccountOrganisationDto>
        {
            new UserAccountOrganisationDto
            {
                UserAccount = default!,
                Organisation = new OrganisationDto
                {
                    Id = 2,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }
        };

        UserAccount entity = mapper.Map<UserAccount>(userAccountDto);
        contextMock.UserAccounts.Add(entity);
        contextMock.SaveChanges();

        var eventData = new[]
        {
            new
            {
                Id = Guid.NewGuid(),
                EventType = "UserAccountDto",
                Subject = "Unit Test",
                EventTime = DateTime.UtcNow,
                Data = userAccountDto
            }
        };

        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(eventData);
        var command = CreateCommand(CreateHttpContext(requestBody));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        // Verify that the ProcessUserAccount method was called with the correct parameters
        mediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserAccountCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateUserAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }


    // Helper method to create a new instance of ProcessUserGidEventCommandHandler
    private ProcessUserGidEventCommandHandler CreateHandler(
        ApplicationDbContext context = default!,
        ISender mediator = default!,
        ILogger<ProcessUserGidEventCommandHandler> logger = default!)
    {
        var myProfile = new AutoMappingProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);

        context ??= GetApplicationDbContext();
        mediator ??= Mock.Of<ISender>();
        logger ??= Mock.Of<ILogger<ProcessUserGidEventCommandHandler>>();

        return new ProcessUserGidEventCommandHandler(context, mediator, mapper, logger);
    }

    // Helper method to create a new instance of HttpContext with the specified request body
    private HttpContext CreateHttpContext(string requestBody)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody));
        return httpContext;
    }

    // Helper method to create a new instance of ProcessUserGidEventCommand with the specified HttpContext
    private ProcessGidEventCommand CreateCommand(HttpContext httpContext)
    {
        return new ProcessGidEventCommand(httpContext);
    }
}