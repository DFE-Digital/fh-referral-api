using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.CreateUserAccount;

public class CreateUserAccountsCommand : IRequest<bool>, ICreateUserAccountsCommand
{
    public CreateUserAccountsCommand(List<UserAccountDto> userAccounts)
    {
        UserAccounts = userAccounts;
    }

    public List<UserAccountDto> UserAccounts { get; }
}

public class CreateUserAccountsCommandHandler : BaseUserAccountHandler, IRequestHandler<CreateUserAccountsCommand, bool>
{
    
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserAccountsCommandHandler> _logger;
    public CreateUserAccountsCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<CreateUserAccountsCommandHandler> logger)
        : base(context)
    {
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> Handle(CreateUserAccountsCommand request, CancellationToken cancellationToken)
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

    private async Task<bool> CreateAndUpdateUserAccounts(CreateUserAccountsCommand request, CancellationToken cancellationToken)
    {
        

        foreach (var account in request.UserAccounts)
        {
            UserAccount entity = _mapper.Map<UserAccount>(account);
            ArgumentNullException.ThrowIfNull(entity);

            entity.OrganisationUserAccounts = _mapper.Map<List<UserAccountOrganisation>>(account.OrganisationUserAccountDtos);

            entity = await AttatchExistingUserAccountRoles(entity, cancellationToken);
            entity = await AttatchExistingOrgansiation(entity, cancellationToken);

            _context.UserAccounts.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            if (entity == null || entity.Id < 1)
            {
                return false;
            }
        }
        
        return true;
    }

    private UserAccount AttatchExistingUserAccountRoles(UserAccount entity)
    {
        //Role? role = _context.Roles.FirstOrDefault(x => x.Id == entity.RoleId);
        //if (role != null)
        //{
        //    entity.Role = role;
        //    entity.RoleId = role.Id;
        //}

        return entity;
    }

    private async Task<UserAccount> AttatchExistingOrgansiation(UserAccount entity, CancellationToken cancellationToken)
    {
        if (entity.OrganisationUserAccounts == null) 
        {
            return entity;
        }
        foreach(UserAccountOrganisation organisationUserAccount in entity.OrganisationUserAccounts)
        {
            Organisation? organisation = _context.Organisations.FirstOrDefault(x => x.Id == organisationUserAccount.Organisation.Id);

            if (organisation == null)
            {
                _context.Organisations.Add(organisationUserAccount.Organisation);
                await _context.SaveChangesAsync(cancellationToken);
                
            }

            organisationUserAccount.Organisation = _context.Organisations.First(x => x.Id == organisationUserAccount.Organisation.Id);
        }

        return entity;
    }
}



