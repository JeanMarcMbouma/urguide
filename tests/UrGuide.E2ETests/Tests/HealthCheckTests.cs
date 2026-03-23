using FluentAssertions;
using UrGuide.E2ETests.Infrastructure;

namespace UrGuide.E2ETests.Tests;

/// <summary>
/// E2E tests for health check endpoints.
/// These tests require a running application instance.
/// Set the E2E_BASE_URL environment variable to point to the running app.
/// </summary>
[Collection("E2E")]
[Trait("Category", "E2E")]
public class HealthCheckTests : BaseE2ETest
{
    public HealthCheckTests(PlaywrightFixture fixture) : base(fixture) { }

    [Fact(Skip = "Requires running application instance")]
    public async Task HealthEndpoint_ReturnsHealthy()
    {
        var response = await Page.GotoAsync($"{BaseUrl}/health");
        response!.Status.Should().Be(200);
    }

    [Fact(Skip = "Requires running application instance")]
    public async Task AliveEndpoint_ReturnsOk()
    {
        var response = await Page.GotoAsync($"{BaseUrl}/alive");
        response!.Status.Should().Be(200);
    }
}
