using Ardalis.GuardClauses;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Interfaces.Commands;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using MediatR;

namespace FamilyHubs.ReferralApi.Api.Commands.UpdateReferral;

public class UpdateReferralCommand : IRequest<string>, IUpdateReferralCommand
{
    public UpdateReferralCommand(string id, ReferralDto referralDto)
    {
        Id = id;
        ReferralDto = referralDto;
    }

    public string Id { get; }
    public ReferralDto ReferralDto { get; }
}

public class UpdateReferralCommandHandler : IRequestHandler<UpdateReferralCommand, string>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateReferralCommandHandler> _logger;
    public UpdateReferralCommandHandler(ApplicationDbContext context, ILogger<UpdateReferralCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<string> Handle(UpdateReferralCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.Referrals.FirstOrDefault(x => x.Id == request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.Id);
        }

        try
        {
            entity.OrganisationId = request.ReferralDto.OrganisationId;
            entity.ServiceId = request.ReferralDto.ServiceId;
            entity.ServiceName = request.ReferralDto.ServiceName;
            entity.ServiceDescription = request.ReferralDto.ServiceDescription;
            entity.ServiceAsJson = request.ReferralDto.ServiceAsJson;
            entity.Referrer = request.ReferralDto.Referrer;
            entity.FullName = request.ReferralDto.FullName;
            entity.HasSpecialNeeds = request.ReferralDto.HasSpecialNeeds;
            entity.Email = request.ReferralDto.Email ?? string.Empty;
            entity.Phone = request.ReferralDto.Phone ?? string.Empty;
            entity.Text = request.ReferralDto.Text ?? string.Empty;
            entity.ReasonForSupport = request.ReferralDto.ReasonForSupport;
            entity.ReasonForRejection = request.ReferralDto.ReasonForRejection;
            //Add Date Received

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

        if (request is not null && request.ReferralDto is not null)
            return request.ReferralDto.Id;
        else
            return string.Empty;
    }
}

