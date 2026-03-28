using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Services.Disputes;
using UrGuide.Services.Payments;
using UrGuide.Services.Reports;

namespace UrGuide.UnitTests.Services;

public class ServiceConstructorTests
{
    [Fact]
    public void DisputeService_throws_when_context_is_null()
    {
        var logger = new LoggerFactory().CreateLogger<DisputeService>();

        var act = () => new DisputeService(null!, logger);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("context");
    }

    [Fact]
    public void DisputeService_throws_when_logger_is_null()
    {
        var act = () => new DisputeService(CreateMockContext(), null!);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("logger");
    }

    [Fact]
    public void ReportingService_throws_when_context_is_null()
    {
        var logger = new LoggerFactory().CreateLogger<ReportingService>();

        var act = () => new ReportingService(null!, logger);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("context");
    }

    [Fact]
    public void ReportingService_throws_when_logger_is_null()
    {
        var act = () => new ReportingService(CreateMockContext(), null!);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("logger");
    }

    [Fact]
    public void PaymentService_throws_when_context_is_null()
    {
        var config = new ConfigurationBuilder().Build();
        var logger = new LoggerFactory().CreateLogger<PaymentService>();

        var act = () => new PaymentService(null!, config, logger);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("context");
    }

    [Fact]
    public void PaymentService_throws_when_configuration_is_null()
    {
        var logger = new LoggerFactory().CreateLogger<PaymentService>();

        var act = () => new PaymentService(CreateMockContext(), null!, logger);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configuration");
    }

    [Fact]
    public void PaymentService_throws_when_logger_is_null()
    {
        var config = new ConfigurationBuilder().Build();

        var act = () => new PaymentService(CreateMockContext(), config, null!);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("logger");
    }

    [Fact]
    public void PayoutService_throws_when_context_is_null()
    {
        var config = new ConfigurationBuilder().Build();
        var logger = new LoggerFactory().CreateLogger<PayoutService>();

        var act = () => new PayoutService(null!, config, logger);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("context");
    }

    [Fact]
    public void PayoutService_throws_when_configuration_is_null()
    {
        var logger = new LoggerFactory().CreateLogger<PayoutService>();

        var act = () => new PayoutService(CreateMockContext(), null!, logger);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configuration");
    }

    [Fact]
    public void PayoutService_throws_when_logger_is_null()
    {
        var config = new ConfigurationBuilder().Build();

        var act = () => new PayoutService(CreateMockContext(), config, null!);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("logger");
    }

    [Fact]
    public void RefundService_throws_when_context_is_null()
    {
        var config = new ConfigurationBuilder().Build();
        var logger = new LoggerFactory().CreateLogger<RefundService>();

        var act = () => new RefundService(null!, config, logger);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("context");
    }

    [Fact]
    public void RefundService_throws_when_configuration_is_null()
    {
        var logger = new LoggerFactory().CreateLogger<RefundService>();

        var act = () => new RefundService(CreateMockContext(), null!, logger);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configuration");
    }

    [Fact]
    public void RefundService_throws_when_logger_is_null()
    {
        var config = new ConfigurationBuilder().Build();

        var act = () => new RefundService(CreateMockContext(), config, null!);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("logger");
    }

    /// <summary>
    /// Helper to create a minimal UrGuideContext for constructor testing.
    /// Uses in-memory provider to avoid real database dependency.
    /// </summary>
    private static UrGuideContext CreateMockContext()
    {
        var options = new DbContextOptionsBuilder<UrGuideContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new UrGuideContext(options);
    }
}
