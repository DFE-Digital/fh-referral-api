using Ardalis.GuardClauses;
using Ardalis.Specification;
using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
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
            if (entity.Status.Id != request.ReferralDto.Status.Id || entity.Referrer.Id != request.ReferralDto.ReferrerDto.Id) 
            {
                _context.Referrals.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
                entity = _mapper.Map<Data.Entities.Referral>(request.ReferralDto);
                entity = AttachExistingStatus(entity);
                _context.Referrals.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }

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
            .Include(x => x.Referrer)
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

    private Data.Entities.Referral AttachExistingStatus(Data.Entities.Referral entity)
    {
        ReferralStatus? referralStatus = _context.ReferralStatuses.SingleOrDefault(x => x.Name == entity.Status.Name);
        if (referralStatus != null)
        {
            entity.StatusId = referralStatus.Id;
            entity.Status = referralStatus;
        }

        Recipient? recipient = _context.Recipients.SingleOrDefault(x => x.Id == entity.Recipient.Id);
        if (recipient != null)
        {
            entity.Recipient = recipient;
        }

        Data.Entities.ReferralService referralService = _context.ReferralServices.SingleOrDefault(x => x.Id == entity.ReferralService.Id);
        if (referralService != null) 
        { 
            entity.ReferralService = referralService;
        }

        Referrer? referrer = _context.Referrers.SingleOrDefault(x => x.Id == entity.Referrer.Id);
        if (referrer != null)
        {
            entity.Referrer = referrer;
        }

        return entity;
    }
}

