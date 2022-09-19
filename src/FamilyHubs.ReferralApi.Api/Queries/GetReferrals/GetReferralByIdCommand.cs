using Ardalis.GuardClauses;
using FamilyHubs.ServiceDirectoryCaseManagement.Common.Dto;
using FamilyHubs.ServiceDirectoryCaseManagement.Core.Entities;
using FamilyHubs.ServiceDirectoryCaseManagement.Infra.Persistence.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralByIdCommand : IRequest<ReferralDto>
{
    public GetReferralByIdCommand(string id)
    {
        Id = id;
    }

    public string Id { get; set; }

}

public class GetReferralByIdCommandHandler : IRequestHandler<GetReferralByIdCommand, ReferralDto>
{
    private readonly ApplicationDbContext _context;

    public GetReferralByIdCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ReferralDto> Handle(GetReferralByIdCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Referrals
            .Include(x => x.Status)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Referral), request.Id);
        }

        List<ReferralStatusDto> referralStatusDtos = new();
        foreach (var status in entity.Status)
        {
            referralStatusDtos.Add(new ReferralStatusDto(status.Id, status.Status));
        }

        var result = new ReferralDto(
           entity.Id,
           entity.OrganisationId,
           entity.ServiceId,
           entity.ServiceName,
           entity.ServiceDescription,
           entity.ServiceAsJson,
           entity.Referrer,
           entity.FullName,
           entity.HasSpecialNeeds,
           entity.Email,
           entity.Phone,
           entity.ReasonForSupport,
           referralStatusDtos
           );

        return result;
    }
}

