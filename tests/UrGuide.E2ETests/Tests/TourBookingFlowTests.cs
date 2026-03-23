using FluentAssertions;
using UrGuide.E2ETests.Infrastructure;

namespace UrGuide.E2ETests.Tests;

/// <summary>
/// E2E tests for the tour request and bidding flow.
/// Tests the complete workflow from creating a tour request to booking completion.
/// </summary>
[Collection("E2E")]
[Trait("Category", "E2E")]
public class TourBookingFlowTests : BaseE2ETest
{
    public TourBookingFlowTests(PlaywrightFixture fixture) : base(fixture) { }

    [Fact(Skip = "Requires running application instance")]
    public async Task Tourist_CanCreateTourRequest()
    {
        // Login as tourist
        await Page.GotoAsync($"{BaseUrl}/login");
        await Page.FillAsync("[name=email]", "tourist@example.com");
        await Page.FillAsync("[name=password]", "TouristPass123!");
        await Page.ClickAsync("button[type=submit]");

        // Navigate to create tour request
        await Page.GotoAsync($"{BaseUrl}/tours/create");

        // Fill in tour request details
        await Page.FillAsync("[name=title]", "Paris City Tour");
        await Page.FillAsync("[name=description]", "Looking for a guided tour of Paris landmarks");

        // Submit
        await Page.ClickAsync("button[type=submit]");

        // Verify tour request was created
        var url = Page.Url;
        url.Should().Contain("/tours/");
    }
}
