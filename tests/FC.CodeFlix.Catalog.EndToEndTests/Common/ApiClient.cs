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

        TOutPut? outputObject = null;
        var outputString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(outputString))
        {
            outputObject = JsonSerializer.Deserialize<TOutPut>(
                outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }

        return (response!, outputObject!);
    }
}
