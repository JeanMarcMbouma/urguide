using FluentAssertions;
using UrGuide.E2ETests.Infrastructure;

namespace UrGuide.E2ETests.Tests;

/// <summary>
/// E2E tests for API documentation endpoints.
/// </summary>
[Collection("E2E")]
[Trait("Category", "E2E")]
public class SwaggerEndpointTests : BaseE2ETest
{
    public SwaggerEndpointTests(PlaywrightFixture fixture) : base(fixture) { }

    [Fact(Skip = "Requires running application instance")]
    public async Task SwaggerUI_IsAccessible()
    {
        var response = await Page.GotoAsync($"{BaseUrl}/swagger/index.html");
        response!.Status.Should().Be(200);

        var title = await Page.TitleAsync();
        title.Should().Contain("Swagger");
    }

    [Fact(Skip = "Requires running application instance")]
    public async Task SwaggerJson_IsAccessible()
    {
        var response = await Page.GotoAsync($"{BaseUrl}/swagger/v1/swagger.json");
        response!.Status.Should().Be(200);

        var body = await response.TextAsync();
        body.Should().Contain("UrGuide API");
    }
}
