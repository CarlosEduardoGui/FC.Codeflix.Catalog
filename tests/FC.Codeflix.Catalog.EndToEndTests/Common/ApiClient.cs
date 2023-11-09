using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;
public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient) =>
        _httpClient = httpClient;

    public async Task<(HttpResponseMessage message, TOutPut output)> Post<TOutPut>
        (string route, object payload)
        where TOutPut : class
    {
        var response = await _httpClient.PostAsync(
            route,
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        );

        TOutPut? output = await GetContentObject<TOutPut>(response);

        return (response!, output!);
    }

    public async Task<(HttpResponseMessage message, TOutPut output)> Get<TOutPut>
        (
            string route,
            object? queryStringParametersObject = null
        )
        where TOutPut : class
    {
        var newQuery = PrepareGetRoute(route, queryStringParametersObject);
        var response = await _httpClient.GetAsync(
            newQuery
        );

        TOutPut? output = await GetContentObject<TOutPut>(response);

        return (response!, output!);
    }

    public async Task<(HttpResponseMessage message, TOutPut output)> Delete<TOutPut>
        (string route)
        where TOutPut : class
    {
        var response = await _httpClient.DeleteAsync(
            route
        );

        TOutPut? output = await GetContentObject<TOutPut>(response);

        return (response!, output!);
    }

    public async Task<(HttpResponseMessage message, TOutPut output)> Put<TOutPut>
        (string route, object payload)
        where TOutPut : class
    {
        var response = await _httpClient.PutAsync(
            route,
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        );

        TOutPut? output = await GetContentObject<TOutPut>(response);

        return (response!, output!);
    }

    private static async Task<TOutPut?> GetContentObject<TOutPut>(HttpResponseMessage response) where TOutPut : class
    {
        var outputString = await response.Content.ReadAsStringAsync();
        TOutPut? output = null;
        if (!string.IsNullOrWhiteSpace(outputString))
            output = JsonSerializer.Deserialize<TOutPut>(outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        return output;
    }

    private string PrepareGetRoute(string route, object? queryStringParametersObject = null)
    {
        if (queryStringParametersObject is null)
            return route;

        var parametersJon = JsonSerializer.Serialize(queryStringParametersObject);
        var parametersDictionary = Newtonsoft.Json.JsonConvert
            .DeserializeObject<Dictionary<string, string>>(parametersJon);

        var url = QueryHelpers.AddQueryString(route, parametersDictionary!);

        return url;
    }
}
