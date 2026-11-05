using System.Net.Http.Json;

namespace RequestImage.Service;

public class ApiHttpClient(HttpClient apiHttpClient) : IHttpClient
{
    public async Task<T?> GetAsync<T>(string url)
    {
        using var response = await apiHttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }
}