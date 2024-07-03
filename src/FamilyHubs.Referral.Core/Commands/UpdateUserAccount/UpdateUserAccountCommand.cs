using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.UpdateUserAccount;

public class UpdateUserAccountCommand : IRequest<bool>, IUpdateUserAccountCommand
{
    public UpdateUserAccountCommand(long userAccountId, UserAccountDto userAccount)
    {
        UserAccountId = userAccountId;
        UserAccount = userAccount;
    }

    public long UserAccountId { get; }

    public UserAccountDto UserAccount { get; }
}

public class UpdateUserAccountCommandHandler : BaseUserAccountHandler, IRequestHandler<UpdateUserAccountCommand, bool>
{
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserAccountCommandHandler> _logger;
    public UpdateUserAccountCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<UpdateUserAccountCommandHandler> logger)
        : base(context)
    {
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateUserAccountCommand request, CancellationToken cancellationToken)
    {
        bool result;
        if (_context.Database.IsSqlServer())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                result = await UpdateAndUpdateUserAccount(request, cancellationToken);
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
                result = await UpdateAndUpdateUserAccount(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }


        return result;
    }

    private async Task<bool> UpdateAndUpdateUserAccount(UpdateUserAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.UserAccounts
            .Include(x => x.OrganisationUserAccounts)
            .FirstOrDefault(x => x.Id == request.UserAccountId);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.UserAccountId.ToString());
        }

        entity = _mapper.Map<UserAccount>(request.UserAccount);
        ArgumentNullException.ThrowIfNull(entity);

        entity.OrganisationUserAccounts = _mapper.Map<List<UserAccountOrganisation>>(request.UserAccount.OrganisationUserAccounts);

        entity = await AttatchExistingUserAccountRoles(entity, cancellationToken);
        entity = await AttatchExistingService(entity, cancellationToken);
        entity = await AttatchExistingOrgansiation(entity, cancellationToken);

        entity.Name = request.UserAccount.Name;
        entity.PhoneNumber = request.UserAccount.PhoneNumber; 
        entity.EmailAddress = request.UserAccount.EmailAddress;
        entity.Team = request.UserAccount.Team;
        
        
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
