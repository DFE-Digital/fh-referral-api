using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.CreateReferral;

public class CreateReferralCommand : IRequest<long>, ICreateReferralCommand
{
    public CreateReferralCommand(ReferralDto referralDto)
    {
        ReferralDto = referralDto;
    }

    public ReferralDto ReferralDto { get; }
}

public class CreateReferralCommandHandler : IRequestHandler<CreateReferralCommand, long>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateReferralCommandHandler> _logger;
    public CreateReferralCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<CreateReferralCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateReferralCommand request, CancellationToken cancellationToken)
    {
        long id;
        if (_context.Database.IsSqlServer())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                id = await CreateAndUpdateReferral(request, cancellationToken);
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
                id = await CreateAndUpdateReferral(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }
            

        return id;
    }

    private async Task<long> CreateAndUpdateReferral(CreateReferralCommand request, CancellationToken cancellationToken)
    {
        Data.Entities.Referral entity = _mapper.Map<Data.Entities.Referral>(request.ReferralDto);
        ArgumentNullException.ThrowIfNull(entity);

        entity.Recipient.Id = 0;

        entity = AttachExistingStatus(entity);
        entity = AttachExistingUserAccount(entity);
        entity = AttachExistingService(entity);

        _context.Referrals.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        ////Make sure all Dto Id's are correctly updated
        //request.ReferralDto.Id = entity.Id;
        //request.ReferralDto.Status.Id = entity.Status.Id;
        //request.ReferralDto.RecipientDto.Id = entity.Recipient.Id;
        //request.ReferralDto.ReferralUserAccountDto.Id = entity.UserAccount.Id;
        //request.ReferralDto.ReferralServiceDto.Id = entity.ReferralService.Id;
        //request.ReferralDto.ReferralServiceDto.OrganisationDto.Id = entity.ReferralService.Organisation.Id;
        
        ////Update Referrer / Recipient / Service / Organisation with latest details
        //entity = _mapper.Map(request.ReferralDto, entity);
        //entity = AttachExistingUserAccount(entity);

        //await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
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
        UserAccount? professional = _context.UserAccounts.SingleOrDefault(x => x.EmailAddress == entity.UserAccount.EmailAddress);
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
    private Data.Entities.Referral AttachExistingService(Data.Entities.Referral entity)
    {
        Data.Entities.ReferralService? referralService = _context.ReferralServices.SingleOrDefault(x => x.Id == entity.ReferralService.Id);
        if (referralService != null)
        {
            entity.ReferralService = referralService;
        }
        return entity;
    }
}

