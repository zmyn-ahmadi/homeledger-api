using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HomeLedger.API.IntegrationTests;

public class SmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task App_starts_and_responds()
    {
        var response = await _client.GetAsync("/swagger");
        // We don't care what status — just that the app booted and didn't crash.
        // 200, 301, 404 are all "alive". 500 would mean startup failure.
        ((int)response.StatusCode)
            .Should()
            .BeLessThan(500);
    }
}
