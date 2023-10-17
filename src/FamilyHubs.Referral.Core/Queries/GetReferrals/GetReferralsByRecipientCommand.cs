using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;



public class GetReferralsByRecipientCommand : IRequest<List<ReferralDto>>
{
    public GetReferralsByRecipientCommand(long organisationId, string? email, string? telephone, string? textphone, string? name, string? postcode)
    {
        OrganisationId = organisationId;
        Email = email;
        Telephone = telephone;
        Textphone = textphone;
        Name = name;
        Postcode = postcode;
    }

    public long OrganisationId { get; }
    public string? Email { get; }
    public string? Telephone { get; }
    public string? Textphone { get; }
    public string? Name { get; }
    public string? Postcode { get; }

}

public class GetReferralsByRecipientHandler : IRequestHandler<GetReferralsByRecipientCommand, List<ReferralDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReferralsByRecipientHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<ReferralDto>> Handle(GetReferralsByRecipientCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Email) &&
            string.IsNullOrEmpty(request.Telephone) &&
            string.IsNullOrEmpty(request.Textphone) &&
            string.IsNullOrEmpty(request.Name) &&
            string.IsNullOrEmpty(request.Postcode)
            ) 
        {
            throw new NotFoundException(nameof(Referral), "Name");
        }

        int pageNumber = 0;
        int currentCount = 0;
        List<Recipient> recipients = new List<Recipient>();
        List<ReferralDto> results = new List<ReferralDto>();

        do
        {
            var entities = await _context.Recipients.Skip(pageNumber).Take(1000)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return results;
            }

            pageNumber++;
            currentCount = entities.Count;

            if (!string.IsNullOrEmpty(request.Email))
            {
                entities = entities.Where(x => x.Email!.ToLower() == request.Email.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(request.Telephone))
            {
                entities = entities.Where(x => x.Telephone == request.Telephone).ToList();
            }
            else if (!string.IsNullOrEmpty(request.Textphone))
            {
                entities = entities.Where(x => x.TextPhone == request.Textphone).ToList();
            }
            else if (!string.IsNullOrEmpty(request.Name) && !string.IsNullOrEmpty(request.Postcode))
            {
                entities = entities.Where(x => x.Name.ToLower() == request.Name.ToLower() && x.PostCode!.ToUpper() == request.Postcode.ToUpper()).ToList();
            }

            if (entities.Any())
            {
                recipients.AddRange(entities);
            }

        } while (currentCount > 999);

        if (recipients.Any()) 
        {
            List<long> recipientIds = recipients.Select(x => x.Id).ToList();

            var entities = await _context.Referrals.GetAll()
                .AsNoTracking()
                .Where(x => recipientIds.Contains(x.RecipientId)).ToListAsync(cancellationToken);

            if (entities.Any()) 
            {
                var mappedList = entities.AsQueryable().ProjectTo<ReferralDto>(_mapper.ConfigurationProvider);
                if (mappedList.Any())
                {
                    results.AddRange(mappedList);
                }
            }

        }

        return results;
    }
}


