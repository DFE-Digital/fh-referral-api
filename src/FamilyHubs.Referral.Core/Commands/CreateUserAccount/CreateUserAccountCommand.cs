﻿using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.CreateUserAccount;

public class CreateUserAccountCommand : IRequest<bool>, ICreateUserAccountCommand
{
    public CreateUserAccountCommand(UserAccountDto userAccount)
    {
        UserAccount = userAccount;
    }

    public UserAccountDto UserAccount { get; }
}

public class CreateUserAccountCommandHandler : BaseUserAccountHandler, IRequestHandler<CreateUserAccountCommand, bool>
{
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserAccountCommandHandler> _logger;
    public CreateUserAccountCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<CreateUserAccountCommandHandler> logger)
        : base(context)
    {
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> Handle(CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        bool result;
        if (_context.Database.IsSqlServer())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                result = await CreateAndUpdateUserAccount(request, cancellationToken);
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
                result = await CreateAndUpdateUserAccount(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }

        return result;
    }

    private async Task<bool> CreateAndUpdateUserAccount(CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        UserAccount entity = _mapper.Map<UserAccount>(request.UserAccount);
        ArgumentNullException.ThrowIfNull(entity);

        entity.OrganisationUserAccounts = _mapper.Map<List<UserAccountOrganisation>>(request.UserAccount.OrganisationUserAccountDtos);

        entity = await AttatchExistingUserAccountRoles(entity, cancellationToken);
        entity = await AttatchExistingOrgansiation(entity, cancellationToken);

        _context.UserAccounts.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        if (entity == null || entity.Id < 1)
        {
            return false;
        }
        

        return true;
    }
}


