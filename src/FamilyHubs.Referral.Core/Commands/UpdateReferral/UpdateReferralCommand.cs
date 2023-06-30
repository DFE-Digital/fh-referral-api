using Ardalis.GuardClauses;
using Ardalis.Specification;
using AutoMapper;
using Azure.Core;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

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
            await UpdateReferrer(entity, request, cancellationToken);
            await UpdateRecipient(entity, request, cancellationToken);
            await UpdateReferralService(entity, request, cancellationToken);

            entity = GetReferral(request);

            entity = _mapper.Map(request.ReferralDto, entity);
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
        var entity = _context.Referrals
            .Include(x => x.Status)
            .Include(x => x.ReferralUserAccount)
            .Include(x => x.Recipient)
            .Include(x => x.ReferralService)
            .ThenInclude(x => x.ReferralOrganisation)
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
            var updatedStatus = _context.ReferralStatuses.SingleOrDefault(x => x.Name == request.ReferralDto.Status.Name);

            if (updatedStatus == null)
            {
                throw new NotFoundException(nameof(ReferralStatus), request.ReferralDto.Status.Name);
            }

            entity.StatusId = updatedStatus.Id;
            entity.Status = updatedStatus;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task UpdateReferrer(Data.Entities.Referral entity, UpdateReferralCommand request, CancellationToken cancellationToken)
    {
        if (entity.ReferralUserAccount.Id != request.ReferralDto.ReferrerDto.Id)
        {
            var updatedReferrer = _context.Referrers.SingleOrDefault(x => x.Id == request.ReferralDto.ReferrerDto.Id);

            if (updatedReferrer == null)
            {
                var userAccount = _mapper.Map<ReferralUserAccount>(request.ReferralDto.ReferrerDto);
                userAccount.ReferralOrganisationId = entity.ReferralService.ReferralOrganisation.Id;
                _context.Referrers.Add(userAccount);
                entity.ReferralUserAccountId = request.ReferralDto.ReferrerDto.Id;
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            entity.ReferralUserAccount = updatedReferrer;
            await _context.SaveChangesAsync(cancellationToken);
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

