using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace FamilyHubs.Referral.Core.Commands.CreateReferral;

public class CreateReferralCommand : IRequest<long>, ICreateReferralCommand
{
    public CreateReferralCommand(ReferralDto referralDto)
    {
        ReferralDto = referralDto;
    }

    public ReferralDto ReferralDto { get; }
}

public class CreateReferralCommandHandler : IRequestHandler<CreateReferralCommand, long>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateReferralCommandHandler> _logger;
    public CreateReferralCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<CreateReferralCommandHandler> logger)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }
    public async Task<long> Handle(CreateReferralCommand request, CancellationToken cancellationToken)
    {
        long id = 0;
        try
        {
            id = CreateReferral(request);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

        return id;
    }

    private long CreateReferral(CreateReferralCommand request)
    {
        Data.Entities.Referral entity = _mapper.Map<Data.Entities.Referral>(request.ReferralDto);
        ArgumentNullException.ThrowIfNull(entity);

        entity = AttachExistingTeam(entity);
        entity = AttachExistingReferrer(entity);
        entity = AttachExistingRecipient(entity);
        entity = AttachExistingService(entity);

        _context.Referrals.Add(entity);

        _context.SaveChanges();

        //Team? team = _context.Teams.SingleOrDefault(x => x.Name == request.ReferralDto.ReferrerDto.Team);
        //if (team == null)
        //{
        //    team = new Team
        //    {
        //        OrganisationId = request.ReferralDto.ReferralServiceDto.ReferralOrganisationDto.Id,
        //        //UserId = entity.Id,
        //        Name = request.ReferralDto.ReferrerDto.Team,
        //    };

        //    _context.Teams.Add(team); 
        //    entity.Team = team;
        //    //entity.TeamId = team.Id;
        //}
        //else
        //{
        //    //team.UserId = entity.Id;
        //    team.OrganisationId = request.ReferralDto.ReferralServiceDto.ReferralOrganisationDto.Id;
        //}

        _context.SaveChanges();

        return entity.Id;
    }

    private Data.Entities.Referral AttachExistingTeam(Data.Entities.Referral entity)
    {
        //Teams? team = _context.Teams.SingleOrDefault(x => x.Name == entity.Referrer.Teams);
        //if (team != null)
        //{
        //    //entity.TeamId = team.Id;
        //    entity.Teams = team;
        //}
       
        return entity;
    }

    private Data.Entities.Referral AttachExistingReferrer(Data.Entities.Referral entity)
    {
        User? referrer = _context.Users.FirstOrDefault(x => x.EmailAddress == entity.Referrer.EmailAddress);
        if (referrer != null) 
        {
            entity.Referrer = referrer;
        }
        return entity;
    }

    private Data.Entities.Referral AttachExistingRecipient(Data.Entities.Referral entity)
    {
        Recipient? recipient = null;
        if (!string.IsNullOrEmpty(entity.Recipient.Telephone))
        {
            recipient = _context.Recipients.FirstOrDefault(x => x.Telephone == entity.Recipient.Telephone);
        }
        else if (!string.IsNullOrEmpty(entity.Recipient.TextPhone))
        {
            recipient = _context.Recipients.FirstOrDefault(x => x.TextPhone == entity.Recipient.Telephone);
        }
        else if (!string.IsNullOrEmpty(entity.Recipient.Email))
        {
            recipient = _context.Recipients.FirstOrDefault(x => !string.IsNullOrEmpty(x.Email) && x.Email.ToLower() == entity.Recipient.Email.ToLower());
        }
        else if (!string.IsNullOrEmpty(entity.Recipient.Name) && !string.IsNullOrEmpty(entity.Recipient.PostCode))
        {
            recipient = _context.Recipients.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.PostCode) && x.Name.ToLower() == entity.Recipient.Name.ToLower() && x.PostCode.ToLower() == entity.Recipient.PostCode.ToLower());
        }

        if (recipient != null) 
        {
            entity.Recipient = recipient;
        }


        return entity;
    }

    private Data.Entities.Referral AttachExistingService(Data.Entities.Referral entity)
    {
        Data.Entities.Service? referrer = _context.Services.FirstOrDefault(x => x.Name.ToLower() == entity.Service.Name.ToLower() && x.Organisation.Name.ToLower() == entity.Service.Organisation.Name.ToLower());
        if (referrer != null)
        {
            entity.Service = referrer;
        }
        return entity;
    }
}

