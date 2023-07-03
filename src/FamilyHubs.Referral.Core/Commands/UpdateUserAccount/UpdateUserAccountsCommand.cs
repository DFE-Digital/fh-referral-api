using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.UpdateUserAccount;

public class UpdateUserAccountsCommand : IRequest<bool>, IUpdateUserAccountCommand
{
    public UpdateUserAccountsCommand(List<UserAccountDto> userAccounts)
    {
        UserAccounts = userAccounts;
    }

    public List<UserAccountDto> UserAccounts { get; }
}

public class UpdateUserAccountsCommandHandler : IRequestHandler<UpdateUserAccountsCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserAccountsCommandHandler> _logger;
    public UpdateUserAccountsCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<UpdateUserAccountsCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateUserAccountsCommand request, CancellationToken cancellationToken)
    {
        bool result;
        if (_context.Database.IsSqlServer())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                result = await UpdateAndUpdateUserAccounts(request, cancellationToken);
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
                result = await UpdateAndUpdateUserAccounts(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }


        return result;
    }

    private async Task<bool> UpdateAndUpdateUserAccounts(UpdateUserAccountsCommand request, CancellationToken cancellationToken)
    {


        foreach (var account in request.UserAccounts)
        {
            UserAccount entity = _mapper.Map<UserAccount>(account);
            ArgumentNullException.ThrowIfNull(entity);

            entity.OrganisationUserAccounts = _mapper.Map<List<OrganisationUserAccount>>(account.OrganisationUserAccountDtos);

            entity = await AttatchExistingOrgansiation(entity, cancellationToken);

            _context.UserAccounts.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            if (entity == null || entity.Id < 1)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<UserAccount> AttatchExistingOrgansiation(UserAccount entity, CancellationToken cancellationToken)
    {
        if (entity.OrganisationUserAccounts == null)
        {
            return entity;
        }
        foreach (OrganisationUserAccount organisationUserAccount in entity.OrganisationUserAccounts)
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



