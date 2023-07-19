using Ardalis.GuardClauses;
using AutoMapper;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Core.Queries.GetUserAccounts;

public class GetUserByIdCommand : IRequest<UserAccountDto>
{
    public GetUserByIdCommand(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}

public class GetUserByIdCommandHandler : IRequestHandler<GetUserByIdCommand, UserAccountDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserByIdCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserAccountDto> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.UserAccounts
            .Include(x => x.OrganisationUserAccounts!)
            .ThenInclude(x => x.Organisation)
            .Where(x => x.Id == request.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync();
       
        if (entity == null)
        {
            throw new NotFoundException(nameof(UserAccount), request.Id.ToString());
        }

        return _mapper.Map<UserAccountDto>(entity);
    }
}
