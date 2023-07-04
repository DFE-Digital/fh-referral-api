using FamilyHubs.Referral.Core.Commands.CreateUserAccount;
using FamilyHubs.Referral.Core.Commands.UpdateUserAccount;
using FamilyHubs.Referral.Core.Queries.GetUserAccounts;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.Referral.Integration.Tests;

public class WhenUsingUserAccounts : DataIntegrationTestBase
{
    [Fact]
    public async Task ThenCreateUserAccount()
    {
#pragma warning disable CS8602
        //Assign 
        UserAccountDto userAccountDto = TestDataProvider.GetUserAccount();
        List<UserAccountDto> listUserAccounts = new List<UserAccountDto> { userAccountDto };
        Data.Entities.UserAccount userAccount = Mapper.Map<UserAccount>(userAccountDto);
        userAccount.OrganisationUserAccounts = Mapper.Map<List<OrganisationUserAccount>>(userAccountDto.OrganisationUserAccountDtos);

        CreateUserAccountCommand command = new CreateUserAccountCommand(listUserAccounts);
        CreateUserAccountCommandHandler handler = new CreateUserAccountCommandHandler(TestDbContext, Mapper, new Mock<ILogger<CreateUserAccountCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new CancellationToken());

        //Assert
        result.Should().BeTrue();
        var actualUserAccount = TestDbContext.UserAccounts
            .Include(x => x.OrganisationUserAccounts)
            .FirstAsync(x => x.OrganisationUserAccounts != null && x.OrganisationUserAccounts.Any(x => x.OrganisationId == userAccountDto.OrganisationUserAccountDtos[0].Organisation.Id));

        actualUserAccount.Result.EmailAddress.Should().Be(userAccount.EmailAddress);
        actualUserAccount.Result.PhoneNumber.Should().Be(userAccount.PhoneNumber);
        actualUserAccount?.Result?.OrganisationUserAccounts[0]?.OrganisationId.Should().Be(userAccountDto.OrganisationUserAccountDtos[0].Organisation.Id);
        

#pragma warning restore CS8602
    }

    [Fact]
    public async Task ThenUpdateUserAccount()
    {
#pragma warning disable CS8602
        //Assign 
        UserAccountDto userAccountDto = TestDataProvider.GetUserAccount();
        userAccountDto.EmailAddress = "UpdatedUser@email.com";
        userAccountDto.PhoneNumber = "0161 111 1112";

        List<UserAccountDto> listUserAccounts = new List<UserAccountDto> { userAccountDto };
        Data.Entities.UserAccount userAccount = Mapper.Map<UserAccount>(userAccountDto);
        userAccount.OrganisationUserAccounts = Mapper.Map<List<OrganisationUserAccount>>(userAccountDto.OrganisationUserAccountDtos);
        TestDbContext.Organisations.Add(userAccount.OrganisationUserAccounts[0].Organisation);
        await TestDbContext.SaveChangesAsync();
        userAccount.OrganisationUserAccounts[0].Organisation = TestDbContext.Organisations.First(x => x.Id == userAccount.OrganisationUserAccounts[0].Organisation.Id);
        TestDbContext.UserAccounts.Add(userAccount);
        await TestDbContext.SaveChangesAsync();

        UpdateUserAccountsCommand command = new UpdateUserAccountsCommand(listUserAccounts);
        UpdateUserAccountsCommandHandler handler = new UpdateUserAccountsCommandHandler(TestDbContext, Mapper, new Mock<ILogger<UpdateUserAccountsCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new CancellationToken());

        //Assert
        result.Should().BeTrue();
        var actualUserAccount = TestDbContext.UserAccounts
            .Include(x => x.OrganisationUserAccounts)
            .FirstAsync(x => x.OrganisationUserAccounts != null && x.OrganisationUserAccounts.Any(x => x.OrganisationId == userAccountDto.OrganisationUserAccountDtos[0].Organisation.Id));

        actualUserAccount.Result.EmailAddress.Should().Be(userAccount.EmailAddress);
        actualUserAccount.Result.PhoneNumber.Should().Be(userAccount.PhoneNumber);
        actualUserAccount?.Result?.OrganisationUserAccounts[0]?.OrganisationId.Should().Be(userAccountDto.OrganisationUserAccountDtos[0].Organisation.Id);


#pragma warning restore CS8602
    }

    [Fact]
    public async Task ThenGetUserAccountByOrganisationId()
    {
#pragma warning disable CS8602
        //Assign 
        UserAccountDto userAccountDto = TestDataProvider.GetUserAccount();
        Data.Entities.UserAccount userAccount = Mapper.Map<UserAccount>(userAccountDto);
        userAccount.OrganisationUserAccounts = Mapper.Map<List<OrganisationUserAccount>>(userAccountDto.OrganisationUserAccountDtos);
        TestDbContext.Organisations.Add(userAccount.OrganisationUserAccounts[0].Organisation);
        await TestDbContext.SaveChangesAsync();
        userAccount.OrganisationUserAccounts[0].Organisation = TestDbContext.Organisations.First(x => x.Id == userAccount.OrganisationUserAccounts[0].Organisation.Id);
        TestDbContext.UserAccounts.Add(userAccount);
        await TestDbContext.SaveChangesAsync();

        GetUsersByOrganisationIdCommand command = new GetUsersByOrganisationIdCommand(userAccount.OrganisationUserAccounts[0].Organisation.Id, 1, 10);
        GetUsersByOrganisationIdCommandHandler handler = new GetUsersByOrganisationIdCommandHandler(TestDbContext, Mapper);

        //Act
        var result = await handler.Handle(command, new CancellationToken());

        //Assert
        result.Items.Count.Should().BeGreaterThan(0);
        var actualUserAccount = TestDbContext.UserAccounts
            .Include(x => x.OrganisationUserAccounts)
            .FirstAsync(x => x.OrganisationUserAccounts != null && x.OrganisationUserAccounts.Any(x => x.OrganisationId == userAccountDto.OrganisationUserAccountDtos[0].Organisation.Id));

        actualUserAccount.Result.EmailAddress.Should().Be(userAccount.EmailAddress);
        actualUserAccount.Result.PhoneNumber.Should().Be(userAccount.PhoneNumber);
        actualUserAccount?.Result?.OrganisationUserAccounts[0]?.OrganisationId.Should().Be(userAccountDto.OrganisationUserAccountDtos[0].Organisation.Id);


#pragma warning restore CS8602
    }
}
