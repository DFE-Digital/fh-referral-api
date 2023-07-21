using Microsoft.AspNetCore.Http;

namespace FamilyHubs.Referral.Core.Interfaces;

public interface IProcessUserGidEventCommand
{
    HttpContext HttpContext { get; }
}
