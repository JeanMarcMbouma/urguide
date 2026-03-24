using Microsoft.Playwright;

namespace UrGuide.E2ETests.Infrastructure;

[CollectionDefinition("E2E")]
public class E2ECollection : ICollectionFixture<PlaywrightFixture> { }

public abstract class BaseE2ETest : IAsyncLifetime
{
    protected readonly PlaywrightFixture Fixture;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    // Base URL should be configurable - defaults to local dev
    protected virtual string BaseUrl => Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "http://localhost:5000";

    protected BaseE2ETest(PlaywrightFixture fixture)
    {
        Fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        Context = await Fixture.Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await Context.DisposeAsync();
    }
}
