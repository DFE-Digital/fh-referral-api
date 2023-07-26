using Microsoft.AspNetCore.Http;

namespace FamilyHubs.Referral.Core.Interfaces;

public interface IProcessUserGridEventCommand
{
    HttpContext HttpContext { get; }
}
