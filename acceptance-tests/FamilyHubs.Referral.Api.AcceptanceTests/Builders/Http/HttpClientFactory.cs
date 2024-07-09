using System.Collections.Concurrent;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Builders.Http;

//This static factory ensures that we are using one HttpClient per BaseUrl used in the solution.
//This prevents a large number sockets being left open after the tests are run
public static class HttpClientFactory
{
    private static readonly ConcurrentDictionary<string, HttpClient> HttpClientList = new();

    public static HttpClient GetHttpClientInstance(string baseUrl)
    {
        if (!HttpClientList.ContainsKey(baseUrl))
            HttpClientList.TryAdd(baseUrl, new HttpClient());

        return HttpClientList[baseUrl];
    }
}