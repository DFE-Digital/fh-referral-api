using Ardalis.GuardClauses;
using AutoMapper;
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

        var serviceIds = await _context.Organisations.Where(x => x.Id == request.OrganisationId).Select(x => x.ReferralServiceId).ToListAsync(cancellationToken);

        int pageNumber = 0;
        int currentCount = 0;
        List<ReferralDto> results = new List<ReferralDto>();

        do
        {
            var entities = _context.Referrals.GetAll().Skip(pageNumber).Take(1000)
                .AsNoTracking()
                .Where(x => serviceIds.Contains(x.ReferralServiceId)).ToList();

            pageNumber++;
            currentCount = entities.Count;

            if (!string.IsNullOrEmpty(request.Email))
            {
                entities = entities.Where(x => x.Recipient.Email!.ToLower() == request.Email.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(request.Telephone))
            {
                entities = entities.Where(x => x.Recipient.Telephone == request.Telephone).ToList();
            }
            else if (!string.IsNullOrEmpty(request.Textphone))
            {
                entities = entities.Where(x => x.Recipient.TextPhone == request.Textphone).ToList();
            }
            else if (!string.IsNullOrEmpty(request.Name) && !string.IsNullOrEmpty(request.Postcode))
            {
                entities = entities.Where(x => x.Recipient.Name.ToLower() == request.Name.ToLower() && x.Recipient.PostCode!.ToUpper() == request.Postcode.ToUpper()).ToList();
            }

            var mappedList = _mapper.Map<List<ReferralDto>>(entities);
            if (mappedList.Any())
            {
                results.AddRange(mappedList);
            }
            

        } while(currentCount > 999);

        return results;
    }
}


