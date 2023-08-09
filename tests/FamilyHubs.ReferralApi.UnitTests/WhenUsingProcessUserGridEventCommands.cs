using AutoMapper;
using Azure.Messaging.EventGrid.SystemEvents;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.Commands;
using FamilyHubs.Referral.Core.Commands.CreateOrUpdateOrganisation;
using FamilyHubs.Referral.Core.Commands.CreateOrUpdateService;
using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace FamilyHubs.Referral.UnitTests;

public class WhenUsingProcessUserGridEventCommands : BaseCreateDbUnitTest
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
    public async Task ThenHandle_WithCustomEventDataAsOrganisation_CallsProcessOrganisation()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        mediatorMock.Setup(x => x.Send(It.IsAny<CreateOrUpdateOrganisationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1L);
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
        result.validationResponse.Should().Be("All Done!");
    }

    //
    [Fact]
    public async Task ThenHandle_WithCustomEventDataAsService_CallsProcessService()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        mediatorMock.Setup(x => x.Send(It.IsAny<CreateOrUpdateServiceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1L);
        var contextMock = GetApplicationDbContext();
        var handler = CreateHandler(contextMock, mediatorMock.Object);

        // Set up the user account DTO for a regular event message
        var eventData = new[]
        {
            new
            {
                Id = Guid.NewGuid(),
                EventType = "ReferralServiceDto",
                Subject = "Unit Test",
                EventTime = DateTime.UtcNow,
                Data = new ReferralServiceDto
                {
                    Id = 3,
                    Name = "Test Event Grid Service",
                    Description = "Test Event Grid Service Description",
                    OrganisationDto = new OrganisationDto
                    {
                        Id = 3,
                        ReferralServiceId = 3,
                        Name = "Test Event Grid Organisation",
                        Description = "Test Event Grid Organisation Description",
                    }
                }
            }
        };

        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(eventData);
        var command = CreateCommand(CreateHttpContext(requestBody));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);


        // Assert
        result.Should().NotBeNull();
        result.validationResponse.Should().Be("All Done!");
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


    // Helper method to create a new instance of ProcessUserGridEventCommandHandler
    private ProcessUserGridEventCommandHandler CreateHandler(
        ApplicationDbContext context = default!,
        ISender mediator = default!,
        ILogger<ProcessUserGridEventCommandHandler> logger = default!)
    {
        context ??= GetApplicationDbContext();
        mediator ??= Mock.Of<ISender>();
        logger ??= Mock.Of<ILogger<ProcessUserGridEventCommandHandler>>();

        return new ProcessUserGridEventCommandHandler(context, mediator, logger);
    }

    //
    [Fact]
    public async Task ThenHandle_WithCustomEventDataAsOrganisation_RealPayload_CallsProcessUserAccount()
    {
        // Arrange
        var mediatorMock = new Mock<ISender>();
        mediatorMock.Setup(x => x.Send(It.IsAny<CreateOrUpdateOrganisationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1L);
        var contextMock = GetApplicationDbContext();
        var handler = CreateHandler(contextMock, mediatorMock.Object);


        string Data = "{\"Services\":[],\"Reviews\":[],\"OrganisationType\":2,\"Name\":\"Test organisation2\",\"Description\":\"Test organisation2\",\"AdminAreaCode\":\"E09000026\",\"AssociatedOrganisationId\":3,\"Logo\":null,\"Uri\":null,\"Url\":null,\"Id\":0}";

        OrganisationDto? org = JsonSerializer.Deserialize<OrganisationDto>(Data);

        // Set up the user account DTO for a regular event message
        var eventData = new[]
        {
            new
            {
                Id = "7844f40e-ef4d-483a-8104-25f52344258d",
                EventType = "OrganisationDto",
                Subject = "Organisation",
                EventTime = DateTime.UtcNow,
                Data = org
            }
        };

        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(eventData);
        var command = CreateCommand(CreateHttpContext(requestBody));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);


        // Assert
        result.Should().NotBeNull();
        result.validationResponse.Should().Be("All Done!");
    }

    // Helper method to create a new instance of HttpContext with the specified request body
    private HttpContext CreateHttpContext(string requestBody)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody));
        return httpContext;
    }

    // Helper method to create a new instance of ProcessUserGridEventCommand with the specified HttpContext
    private ProcessGridEventCommand CreateCommand(HttpContext httpContext)
    {
        return new ProcessGridEventCommand(httpContext);
    }
}
