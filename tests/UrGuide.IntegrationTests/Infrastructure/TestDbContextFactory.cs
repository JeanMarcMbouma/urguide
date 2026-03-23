using Microsoft.EntityFrameworkCore;
using UrGuide.Data;

namespace UrGuide.IntegrationTests.Infrastructure;

/// <summary>
/// Creates an in-memory UrGuideContext for integration testing the service layer.
/// </summary>
public static class TestDbContextFactory
{
    public static UrGuideContext CreateInMemoryContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<UrGuideContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new UrGuideContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
