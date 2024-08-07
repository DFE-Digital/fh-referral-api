using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Builders.Http;

public class HttpRequestFactory
{
    public static async Task<HttpResponseMessage> Get(string baseUrl,
        string? path,
        string? authToken,
        Dictionary<string, string>? headers,
        Dictionary<string, string>? parameters)
    {
        return await new HttpRequestBuilder()
            .AddMethod(HttpMethod.Get)
            .AddRequestUri(baseUrl, path)
            .AddBearerToken(authToken)
            .AddCustomHeaders(headers)
            .AddParameters(parameters)
            .SendAsync();
    }

    public static async Task<HttpResponseMessage> Post<TContent>(
        string baseUrl,
        string? path,
        TContent? body,
        string? authToken,
        Dictionary<string, string>? headers,
        Dictionary<string, string>? parameters)
    {
        return await new HttpRequestBuilder()
            .AddMethod(HttpMethod.Post)
            .AddRequestUri(baseUrl, path)
            .AddBearerToken(authToken)
            .AddContent(CreateHttpContent(body))
            .AddCustomHeaders(headers)
            .AddParameters(parameters)
            .SendAsync();
    }

    //Creates HttpContent from the object provided
    private static HttpContent CreateHttpContent<TContent>(TContent content)
    {
        //If the content is empty then create empty HttpContent
        if (EqualityComparer<TContent>.Default.Equals(content, default))
        {
            return new ByteArrayContent(Array.Empty<byte>());
        }
        //if the content is not empty, then create HttpContent with the Accept header set to 'application/json'
        else
        {
            var json = System.Text.Json.JsonSerializer.Serialize(content);
            var result = new ByteArrayContent(Encoding.UTF8.GetBytes(json));
            result.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return result;
        }
    }
}