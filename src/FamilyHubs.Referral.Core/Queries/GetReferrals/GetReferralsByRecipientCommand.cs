using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;



public class GetReferralsByRecipientCommand : IRequest<List<ReferralDto>>
{
    public GetReferralsByRecipientCommand(string? email, string? telephone, string? textphone, string? name, string? postcode)
    {
        Email = email;
        Telephone = telephone;
        Textphone = textphone;
        Name = name;
        Postcode = postcode;
    }

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

        var entities = _context.Referrals.GetAll()
            .AsNoTracking();

        if (!string.IsNullOrEmpty(request.Email))
        {
            entities = entities.Where(x => x.Recipient.Email == request.Email);
        }
        else if (!string.IsNullOrEmpty(request.Telephone))
        {
            entities = entities.Where(x => x.Recipient.Telephone == request.Telephone);
        }
        else if (!string.IsNullOrEmpty(request.Textphone))
        {
            entities = entities.Where(x => x.Recipient.TextPhone == request.Textphone);
        }
        else if (!string.IsNullOrEmpty(request.Name) && !string.IsNullOrEmpty(request.Postcode))
        {
            entities = entities.Where(x => x.Recipient.Name == request.Name && x.Recipient.PostCode == request.Postcode);
        }

        return await entities.ProjectTo<ReferralDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
    }
}


