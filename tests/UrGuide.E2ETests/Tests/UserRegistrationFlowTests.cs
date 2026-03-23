using FluentAssertions;
using UrGuide.E2ETests.Infrastructure;

namespace UrGuide.E2ETests.Tests;

/// <summary>
/// E2E tests for the user registration and login flow.
/// Tests cover the critical user journey from registration to profile completion.
/// </summary>
[Collection("E2E")]
[Trait("Category", "E2E")]
public class UserRegistrationFlowTests : BaseE2ETest
{
    public UserRegistrationFlowTests(PlaywrightFixture fixture) : base(fixture) { }

    [Fact(Skip = "Requires running application instance")]
    public async Task User_CanRegister_WithValidCredentials()
    {
        // Navigate to registration page
        await Page.GotoAsync($"{BaseUrl}/register");

        // Fill in registration form
        await Page.FillAsync("[name=email]", "test@example.com");
        await Page.FillAsync("[name=password]", "TestPassword123!");
        await Page.FillAsync("[name=confirmPassword]", "TestPassword123!");
        await Page.FillAsync("[name=firstName]", "TestFirst");
        await Page.FillAsync("[name=lastName]", "TestLast");

        // Submit form
        await Page.ClickAsync("button[type=submit]");

        // Verify successful registration
        await Page.WaitForURLAsync($"{BaseUrl}/login**");
    }

    [Fact(Skip = "Requires running application instance")]
    public async Task User_CanLogin_AfterRegistration()
    {
        await Page.GotoAsync($"{BaseUrl}/login");

        await Page.FillAsync("[name=email]", "test@example.com");
        await Page.FillAsync("[name=password]", "TestPassword123!");
        await Page.ClickAsync("button[type=submit]");

        // Verify successful login - should redirect to dashboard
        await Page.WaitForURLAsync($"{BaseUrl}/dashboard**");
    }
}
