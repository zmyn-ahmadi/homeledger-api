using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HomeLedger.API.Services.Basiq;

public class BasiqServices(IConfiguration config, HttpClient http, IUnitOfWork _uow)
{
    private string? _accessToken;
    private readonly string? BASE_URL = config["Basiq:BasiqBaseURL"];

    // Step 1: Get a server-side Basiq access token
    private async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken != null)
            return _accessToken;

        var apiKey = config["Basiq:ApiKey"]?.Trim();

        var req = new HttpRequestMessage(HttpMethod.Post, $"{BASE_URL}/token");
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

        var req = new HttpRequestMessage(HttpMethod.Post, "{BASE_URL}/users");
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
            $"{BASE_URL}/users/{basiqUserId}/auth_link"
        );
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        req.Headers.Add("basiq-version", "3.0");
        req.Content = JsonContent.Create(new { redirect = redirectedUri });

        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            throw new Exception($"Basiq error: {error}");
        }

        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("links").GetProperty("public").GetString()!;
    }

    public async Task<string> ConnectUserAsync(int userId)
    {
        var user = await _uow.Users.GetByIdAsync(userId);

        if (user == null)
            throw new Exception("User not found.");

        // validate existing baisq user
        if (!string.IsNullOrEmpty(user.BasiqUserId))
        {
            var exists = await CheckUserExistAsync(user.BasiqUserId);

            if (!exists)
            {
                user.BasiqUserId = "";
                await _uow.CommitAsync();
            }
        }

        // check if basiqUser exists
        if (string.IsNullOrEmpty(user.BasiqUserId))
        {
            user.BasiqUserId = await CreateBasiqUserAsync(user.Email, user.Profile?.Phone ?? "");
            await _uow.CommitAsync();
        }

        // Get the content/auth URL
        return await GetConsentUrlAsync(user.BasiqUserId);
    }

    public async Task<JsonElement> GetConnectionAsync(int userId)
    {
        var user = await _uow.Users.GetByIdAsync(userId);

        if (user == null)
            throw new Exception("User not found.");

        var basiqUserId = user.BasiqUserId;
        if (string.IsNullOrEmpty(basiqUserId))
        {
            throw new Exception($"User with id:{userId} does not have basiq user id:{basiqUserId}");
        }

        var token = await GetAccessTokenAsync();

        var req = new HttpRequestMessage(
            HttpMethod.Get,
            $"{BASE_URL}/users/{basiqUserId}/connections"
        );

        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Headers.Add("basiq-version", "3.0");

        var res = await http.SendAsync(req);

        var raw = await res.Content.ReadAsStringAsync();
        Console.WriteLine($"Basiq connections response: {raw}");

        res.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<JsonElement>(raw);
    }

    // Step 5: Fetch accounts for a user
    public async Task<JsonElement> GetAccountsAsync(int userId)
    {
        // find the user
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found.");

        Console.WriteLine(user);
        var basiqUserId = user.BasiqUserId;
        Console.WriteLine($"Basiq User Id: {basiqUserId}");
        if (string.IsNullOrEmpty(basiqUserId))
        {
            throw new Exception($"User with id:{userId} does not have basiq user id:{basiqUserId}");
        }

        // check connection status
        var connections = await GetConnectionAsync(userId);
        var state = GetConnectionState(connections);

        if (state == null)
            throw new Exception("No bank connection found. User must complete consent.");

        if (
            state.Equals("active", StringComparison.OrdinalIgnoreCase)
            || state.Equals("connected", StringComparison.OrdinalIgnoreCase)
        )
            throw new Exception($"Bank connection exists but is not ready yet. Status: {state}");

        if (
            !(
                state.Equals("active", StringComparison.OrdinalIgnoreCase)
                || state.Equals("connected", StringComparison.OrdinalIgnoreCase)
            )
        )
            throw new Exception($"Bank connection is not usable. Status: {state}");

        // grab the token
        var token = await GetAccessTokenAsync();

        // req message
        var req = new HttpRequestMessage(
            HttpMethod.Get,
            $"{BASE_URL}/users/{basiqUserId}/accounts"
        );
        // req headers
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Headers.Add("basiq-version", "3.0");

        // send the request
        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            throw new Exception($"Basiq fetch accounts error: {error}");
        }

        // format the res to json element
        return await res.Content.ReadFromJsonAsync<JsonElement>();
    }

    // Get all Transactions
    public async Task<JsonElement> GetTransactionsAsync(int userId, int limit = 500)
    {
        // get user
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found.");

        // get user's basiqUserId
        var basiqUserId = user.BasiqUserId;
        if (string.IsNullOrEmpty(basiqUserId))
            throw new Exception($"Basiq user not found for user id: {userId}");

        // check connection status
        var connections = await GetConnectionAsync(userId);
        if (!HasActiveConnection(connections))
        {
            throw new Exception("Bank still syncing or not connected.");
        }

        // get token
        var token = await GetAccessTokenAsync();

        // create a req message
        var req = new HttpRequestMessage(
            HttpMethod.Get,
            $"{BASE_URL}/users/{basiqUserId}/transactions?limit={limit}"
        );

        // add req headers
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Headers.Add("basiq-version", "3.0");

        // send the req
        var res = await http.SendAsync(req);

        return await res.Content.ReadFromJsonAsync<JsonElement>();
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

    public async Task<object> GetConnectionStatusAsync(int userId)
    {
        var connections = await GetConnectionAsync(userId);

        if (
            !connections.TryGetProperty("data", out var data)
            || data.ValueKind != JsonValueKind.Array
        )
            return new
            {
                connected = false,
                status = "none",
                raw = connections,
            };

        if (data.GetArrayLength() == 0)
            return new { connected = false, status = "none" };

        var first = data[0];
        var status = first.TryGetProperty("status", out var s) ? s.GetString() : "unknown";

        return new
        {
            connected = status == "active" || status == "connected",
            status,
            raw = connections,
        };
    }

    private string? GetConnectionState(JsonElement connections)
    {
        if (
            !connections.TryGetProperty("data", out var data)
            || data.ValueKind != JsonValueKind.Array
        )
            return null;

        foreach (var conn in data.EnumerateArray())
        {
            if (conn.TryGetProperty("status", out var statusEl))
                return statusEl.GetString();
        }
        return null;
    }

    private bool HasActiveConnection(JsonElement connections)
    {
        if (
            !connections.TryGetProperty("data", out var data)
            || data.ValueKind != JsonValueKind.Array
        )
            return false;

        foreach (var conn in data.EnumerateArray())
        {
            if (conn.TryGetProperty("status", out var statusEl))
            {
                var status = statusEl.GetString()?.ToLowerInvariant();
                if (status == "active" || status == "connected")
                    return true;

                if (status == "pending" || status == "syncing")
                    Console.WriteLine(status);
            }
        }
        return false;
    }
}
