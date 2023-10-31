using System.Text;
using System.Text.Json;

namespace FC.CodeFlix.Catalog.EndToEndTests.Common;
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
        (string route)
        where TOutPut : class
    {
        var response = await _httpClient.GetAsync(
            route
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
}
