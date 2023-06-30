using AutoMapper;
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
    public CreateUserAccountCommand(List<UserAccountDto> userAccounts)
    {
        UserAccounts = userAccounts;
    }

    public List<UserAccountDto> UserAccounts { get; }
}

public class CreateUserAccountCommandHandler : IRequestHandler<CreateUserAccountCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserAccountCommandHandler> _logger;
    public CreateUserAccountCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<CreateUserAccountCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
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
                result = await CreateAndUpdateUserAccounts(request, cancellationToken);
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
                result = await CreateAndUpdateUserAccounts(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }


        return result;
    }

    private async Task<bool> CreateAndUpdateUserAccounts(CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        foreach (var account in request.UserAccounts)
        {
            UserAccount entity = _mapper.Map<UserAccount>(account);
            ArgumentNullException.ThrowIfNull(entity);

            entity = AttachExistingOrganisation(entity);

            _context.UserAccounts.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            //Update Organisation with latest details
            entity = _mapper.Map(account, entity);

            await _context.SaveChangesAsync(cancellationToken);

            if (entity == null || entity.Id < 1)
            {
                return false;
            }
        }
        
        return true;
    }
    
    private UserAccount AttachExistingOrganisation(UserAccount entity)
    {
        UserAccount? userAccount = _context.UserAccounts.SingleOrDefault(x => x.Id == entity.Id);
        if (userAccount != null && userAccount.OrganisationUserAccounts != null) 
        {
            foreach(OrganisationUserAccount organisationUserAccount in userAccount.OrganisationUserAccounts)
            {
                Organisation? dbOrganisation = _context.Organisations.SingleOrDefault(x => x.Id == organisationUserAccount.OrganisationId);
                if (dbOrganisation != null)
                {
                    organisationUserAccount.Organisation = dbOrganisation;
                    organisationUserAccount.OrganisationId = dbOrganisation.Id;
                }
            }
        }

        return entity;
    }
}



