using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Core.Queries;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.UpdateReferral;

public class UpdateReferralCommand : IRequest<long>, IUpdateReferralCommand
{
    public UpdateReferralCommand(long id, ReferralDto referralDto)
    {
        Id = id;
        ReferralDto = referralDto;
    }

    public long Id { get; }
    public ReferralDto ReferralDto { get; }
}

public class UpdateReferralCommandHandler : IRequestHandler<UpdateReferralCommand, long>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateReferralCommandHandler> _logger;
    public UpdateReferralCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<UpdateReferralCommandHandler> logger)
    {
        _logger = logger;
        _mapper = mapper;
        _context = context;
    }
    public async Task<long> Handle(UpdateReferralCommand request, CancellationToken cancellationToken)
    {

        Data.Entities.Referral entity = GetReferral(request);

        try
        {
            await UpdateStatus(entity, request, cancellationToken);
            await UpdateUserAccount(entity, request, cancellationToken);
            await UpdateRecipient(entity, request, cancellationToken);
            await UpdateReferralService(entity, request, cancellationToken);

            entity = GetReferral(request);

            entity = _mapper.Map(request.ReferralDto, entity);
            await UpdateUserAccount(entity, request, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

        return entity.Id;
    }

    private Data.Entities.Referral GetReferral(UpdateReferralCommand request)
    {
        var entity = _context.Referrals.GetAll()
            .FirstOrDefault(x => x.Id == request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.Id.ToString());
        }

        return entity;
    }

    private async Task UpdateStatus(Data.Entities.Referral entity, UpdateReferralCommand request, CancellationToken cancellationToken)
    {
        if (entity.Status.Id != request.ReferralDto.Status.Id)
        {
            var updatedStatus = _context.Statuses.SingleOrDefault(x => x.Name == request.ReferralDto.Status.Name);

            if (updatedStatus == null)
            {
                throw new NotFoundException(nameof(Status), request.ReferralDto.Status.Name);
            }

            entity.StatusId = updatedStatus.Id;
            entity.Status = updatedStatus;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task UpdateUserAccount(Data.Entities.Referral entity, UpdateReferralCommand request, CancellationToken cancellationToken)
    {
        if (entity.UserAccount.Id != request.ReferralDto.ReferralUserAccountDto.Id)
        {
            var updatedReferrer = _context.UserAccounts.SingleOrDefault(x => x.Id == request.ReferralDto.ReferralUserAccountDto.Id);

            UpdateUserAccountRole(entity);

            if (updatedReferrer == null)
            {
                
                _context.UserAccounts.Add(_mapper.Map<UserAccount>(request.ReferralDto.ReferralUserAccountDto));
                entity.UserAccountId = request.ReferralDto.ReferralUserAccountDto.Id;
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            entity.UserAccount = updatedReferrer;
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            UpdateUserAccountRole(entity);
        }
    }

    private void UpdateUserAccountRole(Data.Entities.Referral entity)
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
                        return;
                    }

                    entity.UserAccount.UserAccountRoles[i].Role = role;
                    entity.UserAccount.UserAccountRoles[i].RoleId = role.Id;
                    entity.UserAccount.UserAccountRoles[i].UserAccountId = entity.UserAccount.Id;
                }
            }
        }
    }

    private async Task UpdateRecipient(Data.Entities.Referral entity, UpdateReferralCommand request, CancellationToken cancellationToken)
    {
        if (entity.Recipient.Id != request.ReferralDto.RecipientDto.Id)
        {
            var updatedRecipient = _context.Recipients.SingleOrDefault(x => x.Id == request.ReferralDto.RecipientDto.Id);

            if (updatedRecipient == null)
            {
                _context.Recipients.Add(_mapper.Map<Recipient>(request.ReferralDto.RecipientDto));
                await _context.SaveChangesAsync(cancellationToken);
                var recipient = await _context.Recipients.SingleOrDefaultAsync(x => x.Email == request.ReferralDto.RecipientDto.Email);
                if (recipient != null)
                {
                    entity.RecipientId = recipient.Id;
                    entity.Recipient = recipient;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return;
            }

            entity.Recipient = updatedRecipient;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task UpdateReferralService(Data.Entities.Referral entity, UpdateReferralCommand request, CancellationToken cancellationToken)
    {
        if (entity.ReferralService.Id != request.ReferralDto.ReferralServiceDto.Id)
        {
            var updatedReferralService = _context.ReferralServices.SingleOrDefault(x => x.Id == request.ReferralDto.ReferralServiceDto.Id);

            if (updatedReferralService == null)
            {

                _context.ReferralServices.Add(_mapper.Map<Data.Entities.ReferralService>(request.ReferralDto.ReferralServiceDto));
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            entity.ReferralService = updatedReferralService;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

}

