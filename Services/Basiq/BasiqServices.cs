using System.Text;
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

        var apiKey = config["Basiq:ApiKey"];
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:"));

        var req = new HttpRequestMessage(HttpMethod.Post, "https://au-api.basiq.io/token");
        req.Headers.Add("Authorization", $"Basic {encoded}");
        req.Headers.Add("basiq-version", "3.0");
        req.Content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("scope", "SERVER_ACCESS"),
        ]);

        var res = await http.SendAsync(req);
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        _accessToken = json.GetProperty("access").GetString()!;
        return _accessToken;
    }

    // Step 2: Create a Basiq user
    public async Task<string> CreateBasiqUserAsync(string email, string mobile)
    {
        var token = await GetAccessTokenAsync();

        var req = new HttpRequestMessage(HttpMethod.Post, "https://au-api.basiq.io/users");
        req.Headers.Add("Authorization", $"Bearer {token}");
        req.Headers.Add("basiq-version", "3.0");
        req.Content = JsonContent.Create(new { email, mobile });

        var res = await http.SendAsync(req);
        res.EnsureSuccessStatusCode();

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
        req.Headers.Add("Authorization", $"Bearer {token}");
        req.Headers.Add("basiq-version", "3.0");
        req.Content = JsonContent.Create(new { mobile = "" });

        var res = await http.SendAsync(req);
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("links").GetProperty("public").GetString();
    }
}
