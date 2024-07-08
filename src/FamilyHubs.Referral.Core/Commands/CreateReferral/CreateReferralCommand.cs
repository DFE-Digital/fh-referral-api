using System.Diagnostics;
using AutoMapper;
using FamilyHubs.Referral.Core.ClientServices;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Entities.Metrics;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.ReferralService.Shared.Models;
using FamilyHubs.SharedKernel.Identity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.CreateReferral;

public record CreateReferralCommand(CreateReferralDto CreateReferral, FamilyHubsUser FamilyHubsUser)
    : IRequest<ReferralResponse>, ICreateReferralCommand;

public class CreateReferralCommandHandler : IRequestHandler<CreateReferralCommand, ReferralResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IServiceDirectoryService _serviceDirectoryService;
    private readonly ILogger<CreateReferralCommandHandler> _logger;

    public CreateReferralCommandHandler(ApplicationDbContext context, IMapper mapper, IServiceDirectoryService serviceDirectoryService, ILogger<CreateReferralCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _serviceDirectoryService = serviceDirectoryService;
    }

    public async Task<ReferralResponse> Handle(CreateReferralCommand request, CancellationToken cancellationToken)
    {
        await WriteCreateReferralMetrics(request);

        //todo: I don't think these explicit transactions are necessary
        ReferralResponse referralResponse;
        if (_context.Database.IsSqlServer())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                referralResponse = await CreateAndUpdateReferral(request, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }
        else
        {
            try
            {
                referralResponse = await CreateAndUpdateReferral(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }
            

        return referralResponse;
    }

    private async Task WriteCreateReferralMetrics(CreateReferralCommand request)
    {
        var metrics = new ConnectionRequestsSentMetric
        {
            OrganisationId = long.Parse(request.FamilyHubsUser.OrganisationId),
            UserAccountId = long.Parse(request.FamilyHubsUser.AccountId),
            RequestTimestamp = request.CreateReferral.Metrics.RequestTimestamp.DateTime,
            RequestCorrelationId = Activity.Current!.TraceId.ToString(),
            ResponseTimestamp = null,
            HttpResponseCode = null,
            ConnectionRequestId = null,
            ConnectionRequestReferenceCode = null
        };

        _context.Add(metrics);
        await _context.SaveChangesAsync();
    }

    private async Task<ReferralResponse> CreateAndUpdateReferral(CreateReferralCommand request, CancellationToken cancellationToken)
    {
        Data.Entities.Referral entity = _mapper.Map<Data.Entities.Referral>(request.CreateReferral.Referral);
        ArgumentNullException.ThrowIfNull(entity);

        entity.Recipient.Id = 0;

        entity = AttachExistingStatus(entity);
        entity = AttachExistingUserAccount(entity);
        entity = await AttachExistingService(entity);

        _context.Referrals.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return new ReferralResponse
        {
            Id = entity.Id,
            ServiceName = entity.ReferralService.Name,
            OrganisationId = entity.ReferralService.Organisation.Id,
        };
    }

    private Data.Entities.Referral AttachExistingStatus(Data.Entities.Referral entity)
    {
        Status? referralStatus = _context.Statuses.SingleOrDefault(x => x.Name == entity.Status.Name);
        if (referralStatus != null)
        {
            entity.Status = referralStatus;
        }
        return entity;
    }

    private Data.Entities.Referral AttachExistingUserAccount(Data.Entities.Referral entity)
    {
        UserAccount? professional = _context.UserAccounts.SingleOrDefault(x => x.Id == entity.UserAccount.Id);
        if (professional != null) 
        {
            entity.UserAccount = professional;
        }
        else
        {
            if (entity.UserAccount != null && entity.UserAccount.UserAccountRoles != null) 
            {
                for (int i = 0; i < entity.UserAccount.UserAccountRoles.Count; i++)
                {
                    Role? role = _context.Roles.SingleOrDefault(x => x.Name == entity.UserAccount.UserAccountRoles[i].Role.Name);
                    if (role != null)
                    {
                        UserAccountRole? userAccountRole = _context.UserAccountRoles.SingleOrDefault(x => x.RoleId == role.Id && x.UserAccountId == entity.UserAccount.Id);
                        if (userAccountRole != null)
                        {
                            entity.UserAccount.UserAccountRoles[i] = userAccountRole;
                            return entity;
                        }

                        entity.UserAccount.UserAccountRoles[i].Role = role;
                        entity.UserAccount.UserAccountRoles[i].RoleId = role.Id;
                        entity.UserAccount.UserAccountRoles[i].UserAccountId = entity.UserAccount.Id;
                    }

                }
            }
            
        }
        return entity;
    }

    private async Task<Data.Entities.Referral> AttachExistingService(Data.Entities.Referral entity)
    {
        Data.Entities.ReferralService? referralService = _context.ReferralServices.SingleOrDefault(x => x.Id == entity.ReferralService.Id);
        if (referralService == null)
        {
            ServiceDirectory.Shared.Dto.ServiceDto? sdService = await _serviceDirectoryService.GetServiceById(entity.ReferralService.Id);
            if (sdService == null)
            {
                throw new ArgumentException($"Failed to return Service from service directory for Id = {entity.ReferralService.Id}");
            }

            // check if the organization already exists
            //todo: do we need to update the organisation from the sd, if it already exists?
            Organisation? organisation = await _context.Organisations.FindAsync(sdService.OrganisationId);
            if (organisation == null)
            {
                ServiceDirectory.Shared.Dto.OrganisationDto? sdOrganisation = await _serviceDirectoryService.GetOrganisationById(sdService.OrganisationId);
                if (sdOrganisation == null)
                {
                    throw new ArgumentException($"Failed to return Organisation from service directory for Id = {sdService.OrganisationId}");
                }

                //todo: Organisation has a ReferralServiceId, but an organisation can have multiple services
                organisation = new Organisation
                {
                    Id = sdOrganisation.Id,
                    Name = sdOrganisation.Name,
                    Description = sdOrganisation.Description
                };
            }

            Data.Entities.ReferralService srv = new Data.Entities.ReferralService
            {
                Id = sdService.Id,
                Name = sdService.Name,
                Description = sdService.Description,
                Organisation = organisation
            };

            _context.ReferralServices.Add(srv);
            await _context.SaveChangesAsync();
            referralService = _context.ReferralServices.SingleOrDefault(x => x.Id == entity.ReferralService.Id);
        }

        if (referralService != null)
        {
            entity.ReferralService = referralService;
        }
        return entity;
    }
}
