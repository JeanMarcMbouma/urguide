using BbQ.MockLite;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UrGuide.WebApp.Controllers;
using UrGuide.WebApp.Resources;
using UrGuide.WebApp.Services;

namespace UrGuide.IntegrationTests.Controllers;

public class SocialAuthControllerTests
{
    private readonly Mock<ISocialAuthService> _socialAuthServiceMock;
    private readonly Mock<ILogger<SocialAuthController>> _loggerMock;
    private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
    private readonly SocialAuthController _controller;

    public SocialAuthControllerTests()
    {
        _socialAuthServiceMock = Mock.Create<ISocialAuthService>();
        _loggerMock = Mock.Create<ILogger<SocialAuthController>>();
        _localizerMock = Mock.Create<IStringLocalizer<SharedResource>>();
        _localizerMock.Setup(l => l[It.IsAny<string>()])
            .Returns(new LocalizedString("SocialAuth_UnsupportedProvider", "Provider not supported"));
        _controller = new SocialAuthController(
            _socialAuthServiceMock.Object,
            _loggerMock.Object,
            _localizerMock.Object);

        // Set up a default HttpContext with a mock URL helper
        var httpContext = new DefaultHttpContext();
        var urlHelperMock = Mock.Create<IUrlHelper>();
        urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/api/social-auth/callback/test");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        _controller.Url = urlHelperMock.Object;
    }

    [Fact]
    public void Login_WithUnsupportedProvider_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Login("Facebook");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Login_WithSupportedProvider_ReturnsChallengeResult()
    {
        // Act
        var result = _controller.Login("Google");

        // Assert
        result.Should().BeOfType<ChallengeResult>();
        var challenge = (ChallengeResult)result;
        challenge.AuthenticationSchemes.Should().Contain("Google");
    }

    [Fact]
    public void Login_WithMicrosoft_ReturnsChallengeResult()
    {
        // Act
        var result = _controller.Login("Microsoft");

        // Assert
        result.Should().BeOfType<ChallengeResult>();
        var challenge = (ChallengeResult)result;
        challenge.AuthenticationSchemes.Should().Contain("Microsoft");
    }

    [Fact]
    public void Login_WithApple_ReturnsChallengeResult()
    {
        // Act
        var result = _controller.Login("Apple");

        // Assert
        result.Should().BeOfType<ChallengeResult>();
        var challenge = (ChallengeResult)result;
        challenge.AuthenticationSchemes.Should().Contain("Apple");
    }

    [Fact]
    public async Task UnlinkProvider_WithUnsupportedProvider_ReturnsBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser("user-1");

        // Act
        var result = await _controller.UnlinkProvider("Facebook", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UnlinkProvider_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange - no user claims set

        // Act
        var result = await _controller.UnlinkProvider("Google", CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UnlinkProvider_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        SetupAuthenticatedUser("user-1");
        _socialAuthServiceMock
            .Setup(s => s.UnlinkProviderAsync("user-1", "Google", It.IsAny<CancellationToken>()))
            .ReturnsAsync(SocialAuthResult.Ok("user-1"));
        _socialAuthServiceMock
            .Setup(s => s.LogAuditEventAsync(
                "user-1", "Google", "Unlinked", null,
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UnlinkProvider("Google", CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UnlinkProvider_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser("user-1");
        _socialAuthServiceMock
            .Setup(s => s.UnlinkProviderAsync("user-1", "Google", It.IsAny<CancellationToken>()))
            .ReturnsAsync(SocialAuthResult.Fail("Cannot unlink the last login method."));

        // Act
        var result = await _controller.UnlinkProvider("Google", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetLinkedProviders_WhenAuthenticated_ReturnsOk()
    {
        // Arrange
        SetupAuthenticatedUser("user-1");
        var providers = new List<SocialLoginProviderDto>
        {
            new() { Provider = "Google", Email = "user@gmail.com", LinkedAt = DateTime.UtcNow },
            new() { Provider = "Microsoft", Email = "user@outlook.com", LinkedAt = DateTime.UtcNow }
        };
        _socialAuthServiceMock
            .Setup(s => s.GetLinkedProvidersAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(providers);

        // Act
        var result = await _controller.GetLinkedProviders(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        var returnedProviders = okResult.Value as IReadOnlyList<SocialLoginProviderDto>;
        returnedProviders.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLinkedProviders_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange - no user claims set

        // Act
        var result = await _controller.GetLinkedProviders(CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetAuditLog_WhenAuthenticated_ReturnsOk()
    {
        // Arrange
        SetupAuthenticatedUser("user-1");
        var logs = new List<SocialLoginAuditLogDto>
        {
            new() { Provider = "Google", Action = "Login", Timestamp = DateTime.UtcNow },
            new() { Provider = "Google", Action = "Linked", Timestamp = DateTime.UtcNow.AddDays(-1) }
        };
        _socialAuthServiceMock
            .Setup(s => s.GetAuditLogAsync("user-1", 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        // Act
        var result = await _controller.GetAuditLog(50, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        var returnedLogs = okResult.Value as IReadOnlyList<SocialLoginAuditLogDto>;
        returnedLogs.Should().HaveCount(2);
    }

    [Fact]
    public void GetAvailableProviders_ReturnsAllThreeProviders()
    {
        // Act
        var result = _controller.GetAvailableProviders();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void InitiateLink_WithUnsupportedProvider_ReturnsBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser("user-1");

        // Act
        var result = _controller.InitiateLink("Facebook");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void InitiateLink_WithSupportedProvider_ReturnsChallengeResult()
    {
        // Arrange
        SetupAuthenticatedUser("user-1");

        // Act
        var result = _controller.InitiateLink("Google");

        // Assert
        result.Should().BeOfType<ChallengeResult>();
    }

    private void SetupAuthenticatedUser(string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, $"{userId}@test.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };
        _controller.ControllerContext.HttpContext = httpContext;
    }
}
