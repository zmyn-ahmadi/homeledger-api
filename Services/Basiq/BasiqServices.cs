using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HomeLedger.API.Services.Basiq;

public class BasiqServices(IConfiguration config, HttpClient http)
{
    private string? _accessToken;

    // Step 1: Get a server-side Basiq access token
    private async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken != null)
            return _accessToken;

        var apiKey = config["Basiq:ApiKey"]?.Trim();

        var req = new HttpRequestMessage(HttpMethod.Post, "https://au-api.basiq.io/token");
        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
        req.Headers.Add("basiq-version", "3.0");
        req.Content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("scope", "SERVER_ACCESS"),
        ]);

        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            throw new Exception($"Basiq error: {error}");
        }

        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        _accessToken =
            json.GetProperty("access_token").GetString()
            ?? throw new Exception("access_token was null in Basiq response");
        return _accessToken;
    }

    // Step 2: Create a Basiq user
    public async Task<string> CreateBasiqUserAsync(string email, string mobile)
    {
        var token = await GetAccessTokenAsync();

        var req = new HttpRequestMessage(HttpMethod.Post, "https://au-api.basiq.io/users");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Headers.Add("basiq-version", "3.0");
        req.Content = JsonContent.Create(new { email, mobile });

        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            throw new Exception($"Basiq error: {error}");
        }

        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetString()!;
    }

    // Step 3: Create a consent URL for the user
    public async Task<string> GetConsentUrlAsync(string basiqUserId)
    {
        var token = await GetAccessTokenAsync();
        var redirectedUri = config["Basiq:RedirectedUri"];

        var req = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://au-api.basiq.io/users/{basiqUserId}/auth_link"
        );
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        req.Headers.Add("basiq-version", "3.0");
        req.Content = JsonContent.Create(new { });

        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            throw new Exception($"Basiq error: {error}");
        }

        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("links").GetProperty("public").GetString();
    }

    // Check if a basiq user exists
    public async Task<bool> CheckUserExistAsync(string basiqUserId)
    {
        var token = await GetAccessTokenAsync();
        try
        {
            var req = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://au-api.basiq.io/users/{basiqUserId}"
            );
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await http.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                return true;
            }

            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            var error = await res.Content.ReadAsStringAsync();
            throw new Exception($"Basiq error: {error}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed checking Basiq user: {ex.Message}");
        }
    }
}
