using AutoMapper;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Core.Commands.CreateOrUpdateOrganisation;

public class CreateOrUpdateOrganisationCommand : IRequest<long>//, ICreateOrUpdateOrganisationCommand
{
    public CreateOrUpdateOrganisationCommand(OrganisationDto organisationDto)
    {
        OrganisationDto = organisationDto;
    }

    public OrganisationDto OrganisationDto { get; }
}

public class CreateOrUpdateOrganisationCommandHandler :  IRequestHandler<CreateOrUpdateOrganisationCommand, long>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrUpdateOrganisationCommandHandler> _logger;
    public CreateOrUpdateOrganisationCommandHandler(ApplicationDbContext context, IMapper mapper, ILogger<CreateOrUpdateOrganisationCommandHandler> logger)
      
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateOrUpdateOrganisationCommand request, CancellationToken cancellationToken)
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

    private async Task<long> CreateOrUpdateOrganisation(CreateOrUpdateOrganisationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Process Custom Event
            _logger.LogInformation("Creating Organisation for Processing Events: {OrganisationName} ", request.OrganisationDto.Name);

            var mappedOrganisation = _mapper.Map<Organisation>(request.OrganisationDto);

            Organisation? organisation = _context.Organisations.FirstOrDefault(x => x.Id == request.OrganisationDto.Id);
            if (organisation != null)
            {
                _mapper.Map(request.OrganisationDto, organisation);
                _logger.LogInformation("Event Grid Found Organisation {OrganisationName} with ID: {OrganisationId}", request.OrganisationDto.Name, request.OrganisationDto.Id);
            }
            else
            {
                _context.Organisations.Add(mappedOrganisation);
                _logger.LogInformation("Event Grid Adding New Organisation {OrganisationName} with ID: {OrganisationId}", request.OrganisationDto.Name, request.OrganisationDto.Id);
            }

            _logger.LogInformation("Saving Changes for Organisation {OrganisationName} with ID: {OrganisationId}", request.OrganisationDto.Name, request.OrganisationDto.Id);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved Changes for Organisation {OrganisationName} with ID: {OrganisationId}", request.OrganisationDto.Name, request.OrganisationDto.Id);

            return mappedOrganisation?.Id ?? 0L;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Event Grid Receive Message - Failed to save organisation");
            return 0L;
        }
    }
}


