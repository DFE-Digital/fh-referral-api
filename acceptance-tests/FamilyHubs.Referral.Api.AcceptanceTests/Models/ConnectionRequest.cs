using System.Net;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Models
{
    public class ConnectionRequest
    {
        public long connectionRequestId { get; set;}
        public HttpStatusCode? httpResponseCode { get; set;}
        public DateTimeOffset requestTimestamp { get; set;}
    }
}