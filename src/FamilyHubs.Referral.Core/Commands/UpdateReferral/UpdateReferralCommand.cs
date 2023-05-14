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
using System.Collections.Generic;

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
        var entity = _context.Referrals
            .Include(x => x.Status)
            .Include(x => x.Referrer)
            .Include(x => x.Recipient)
            .Include(x => x.Service)
            .ThenInclude(x => x.Organisation)
            .FirstOrDefault(x => x.Id == request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.Id.ToString());
        }

        try
        {
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
}

