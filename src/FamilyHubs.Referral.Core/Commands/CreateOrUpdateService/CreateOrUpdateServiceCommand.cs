using AutoMapper;
using FamilyHubs.Referral.Core.Interfaces.Commands;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.CreateOrUpdateService;



public class CreateOrUpdateServiceCommand : IRequest<long>, ICreateOrUpdateServiceCommand
{
    public CreateOrUpdateServiceCommand(ReferralServiceDto referralServiceDto)
    {
        ReferralServiceDto = referralServiceDto;
    }

    public ReferralServiceDto ReferralServiceDto { get; }
}

public class CreateOrUpdateServiceCommandHandler : IRequestHandler<CreateOrUpdateServiceCommand, long>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrUpdateServiceCommandHandler> _logger;
    public CreateOrUpdateServiceCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<CreateOrUpdateServiceCommandHandler> logger)

    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateOrUpdateServiceCommand request, CancellationToken cancellationToken)
    {
        long result = 0;
        if (_context.Database.IsSqlServer())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                result = await CreateOrUpdateOrganisation(request, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }
        else
        {
            try
            {
                result = await CreateOrUpdateOrganisation(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating referral. {exceptionMessage}", ex.Message);
                throw;
            }
        }

        return result;
    }

    private async Task<long> CreateOrUpdateOrganisation(CreateOrUpdateServiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Process Custom Event
            _logger.LogInformation("Creating Organisation for Processing Events: {OrganisationName} ", request.ReferralServiceDto.Name);

            var mappedService = _mapper.Map<Data.Entities.ReferralService>(request.ReferralServiceDto);
            mappedService = AttachExistingOrganisation(mappedService);

            Data.Entities.ReferralService? service = _context.ReferralServices.Include(x => x.Organisation).FirstOrDefault(x => x.Id == request.ReferralServiceDto.Id);
            if (service != null)
            {
                _mapper.Map(request.ReferralServiceDto, service);
                _logger.LogInformation("Event Grid Found Organisation {OrganisationName} with ID: {OrganisationId}", request.ReferralServiceDto.Name, request.ReferralServiceDto.Id);
            }
            else
            { 
                _context.ReferralServices.Add(mappedService);
                _logger.LogInformation("Event Grid Adding New Organisation {OrganisationName} with ID: {OrganisationId}", request.ReferralServiceDto.Name, request.ReferralServiceDto.Id);
            }

            _logger.LogInformation("Saving Changes for Organisation {OrganisationName} with ID: {OrganisationId}", request.ReferralServiceDto.Name, request.ReferralServiceDto.Id);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved Changes for Organisation {OrganisationName} with ID: {OrganisationId}", request.ReferralServiceDto.Name, request.ReferralServiceDto.Id);

            return mappedService?.Id ?? 0L;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Event Grid Receive Message - Failed to save organisation");
            return 0L;
        }
    }

    private Data.Entities.ReferralService AttachExistingOrganisation(Data.Entities.ReferralService entity)
    {
        Data.Entities.Organisation? organisation = _context.Organisations.SingleOrDefault(x => x.Id == entity.Organisation.Id);
        if (organisation != null)
        {
            entity.Organisation = organisation;
            entity.Organisation.Name = entity.Organisation.Name ?? string.Empty;
            entity.Organisation.Description = entity.Organisation.Description ?? string.Empty;
        }
        return entity;
    }
}



