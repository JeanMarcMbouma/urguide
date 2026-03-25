using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using UrGuide.WebApp.Controllers;

namespace UrGuide.IntegrationTests.Controllers;

public class AuthorizationCoverageTests
{
    [Fact]
    public void PostController_Class_ShouldRequireAuthorization()
    {
        typeof(PostController).Should().BeDecoratedWith<AuthorizeAttribute>();
    }

    [Fact]
    public void PostController_PublicEndpoints_ShouldAllowAnonymous()
    {
        typeof(PostController).GetMethod(nameof(PostController.GetUsersPosts))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(PostController).GetMethod(nameof(PostController.SearchPost))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(PostController).GetMethod(nameof(PostController.Get))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(PostController).GetMethod(nameof(PostController.Last100))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(PostController).GetMethod(nameof(PostController.Top10))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(PostController).GetMethod(nameof(PostController.GetOne))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(PostController).GetMethod(nameof(PostController.Top100))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(PostController).GetMethod(nameof(PostController.GetItineraries))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
    }

    [Fact]
    public void TourRequestController_Class_ShouldRequireAuthorization()
    {
        typeof(TourRequestController).Should().BeDecoratedWith<AuthorizeAttribute>();
    }

    [Fact]
    public void TourRequestController_PublicEndpoints_ShouldAllowAnonymous()
    {
        typeof(TourRequestController).GetMethod(nameof(TourRequestController.GetTourRequest))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(TourRequestController).GetMethod(nameof(TourRequestController.GetTourRequests))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(TourRequestController).GetMethod(nameof(TourRequestController.GetTourRequestsByRegion))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
    }

    [Fact]
    public void FeedbackController_Class_ShouldRequireAuthorization()
    {
        typeof(FeedbackController).Should().BeDecoratedWith<AuthorizeAttribute>();
    }

    [Fact]
    public void FeedbackController_PublicEndpoints_ShouldAllowAnonymous()
    {
        typeof(FeedbackController).GetMethod(nameof(FeedbackController.GetUserFeedback))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
        typeof(FeedbackController).GetMethod(nameof(FeedbackController.GetPostFeedback))!.Should().BeDecoratedWith<AllowAnonymousAttribute>();
    }

    [Fact]
    public void LocalizationController_AdminEndpoints_ShouldRequireAdminRole()
    {
        var getTranslationsAuthorize = typeof(LocalizationController).GetMethod(nameof(LocalizationController.GetTranslations))!
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>()
            .Single();
        getTranslationsAuthorize.Roles.Should().Be("Admin");

        var getAllTranslationsAuthorize = typeof(LocalizationController).GetMethod(nameof(LocalizationController.GetAllTranslations))!
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>()
            .Single();
        getAllTranslationsAuthorize.Roles.Should().Be("Admin");
    }

    [Fact]
    public void LocalizationController_SupportedLanguagesEndpoint_ShouldAllowAnonymous()
    {
        typeof(LocalizationController).GetMethod(nameof(LocalizationController.GetSupportedLanguages))!
            .Should().BeDecoratedWith<AllowAnonymousAttribute>();
    }
}
