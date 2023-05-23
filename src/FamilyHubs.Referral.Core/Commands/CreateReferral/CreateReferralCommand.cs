using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

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
            id = await CreateAndUpdateReferral(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
            throw;
        }

        return id;
    }

    private async Task<long> CreateAndUpdateReferral(CreateReferralCommand request, CancellationToken cancellationToken)
    {
        Data.Entities.Referral entity = _mapper.Map<Data.Entities.Referral>(request.ReferralDto);
        ArgumentNullException.ThrowIfNull(entity);

        entity = AttachExistingStatus(entity);
        entity = AttachExistingReferrer(entity);
        entity = AttachExistingRecipient(entity);
        entity = AttachExistingService(entity);

        _context.Referrals.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        //Make sure all Dto Id's are correctly updated
        request.ReferralDto.Id = entity.Id;
        request.ReferralDto.RecipientDto.Id = entity.Recipient.Id;
        request.ReferralDto.ReferrerDto.Id = entity.Referrer.Id;
        request.ReferralDto.ReferralServiceDto.Id = entity.ReferralService.Id;
        request.ReferralDto.ReferralServiceDto.ReferralOrganisationDto.Id = entity.ReferralService.ReferralOrganisation.Id;


        //Update Referrer / Recipient / Service / Organisation with latest details
        entity = _mapper.Map(request.ReferralDto, entity);
        entity = AttachExistingStatus(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private Data.Entities.Referral AttachExistingStatus(Data.Entities.Referral entity)
    {
        ReferralStatus? referralStatus = _context.ReferralStatuses.SingleOrDefault(x => x.Name == entity.Status.Name);
        if (referralStatus != null)
        {
            entity.Status = referralStatus;
        }
        return entity;
    }

    private Data.Entities.Referral AttachExistingReferrer(Data.Entities.Referral entity)
    {
        Referrer? referrer = _context.Referrers.SingleOrDefault(x => x.EmailAddress == entity.Referrer.EmailAddress);
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
            recipient = _context.Recipients.SingleOrDefault(x => x.Telephone == entity.Recipient.Telephone);
        }
        else if (!string.IsNullOrEmpty(entity.Recipient.TextPhone))
        {
            recipient = _context.Recipients.SingleOrDefault(x => x.TextPhone == entity.Recipient.TextPhone);
        }
        else if (!string.IsNullOrEmpty(entity.Recipient.Email))
        {
            recipient = _context.Recipients.SingleOrDefault(x => !string.IsNullOrEmpty(x.Email) && x.Email.ToLower() == entity.Recipient.Email.ToLower());
        }
        else if (!string.IsNullOrEmpty(entity.Recipient.Name) && !string.IsNullOrEmpty(entity.Recipient.PostCode))
        {
            recipient = _context.Recipients.SingleOrDefault(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.PostCode) && x.Name.ToLower() == entity.Recipient.Name.ToLower() && x.PostCode.ToLower() == entity.Recipient.PostCode.ToLower());
        }

        if (recipient != null) 
        {
            entity.Recipient = recipient;
        }


        return entity;
    }

    private Data.Entities.Referral AttachExistingService(Data.Entities.Referral entity)
    {
        Data.Entities.ReferralService? referralService = _context.ReferralServices.SingleOrDefault(x => x.Id == entity.ReferralService.Id);
        if (referralService != null)
        {
            entity.ReferralService = referralService;
        }
        return entity;
    }
}

