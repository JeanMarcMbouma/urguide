using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using UrGuide.WebApp.Controllers;

namespace UrGuide.IntegrationTests.Controllers;

public class AuthorizationCoverageTests
{
    private static void AssertEndpointHasAttribute<TAttribute>(Type controllerType, string methodName)
        where TAttribute : Attribute
    {
        controllerType.GetMethod(methodName)!.Should().BeDecoratedWith<TAttribute>();
    }

    [Fact]
    public void PostController_Class_ShouldRequireAuthorization()
    {
        typeof(PostController).Should().BeDecoratedWith<AuthorizeAttribute>();
    }

    [Fact]
    public void PostController_PublicEndpoints_ShouldAllowAnonymous()
    {
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(PostController), nameof(PostController.GetUsersPosts));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(PostController), nameof(PostController.SearchPost));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(PostController), nameof(PostController.Get));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(PostController), nameof(PostController.Last100));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(PostController), nameof(PostController.Top10));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(PostController), nameof(PostController.GetOne));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(PostController), nameof(PostController.Top100));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(PostController), nameof(PostController.GetItineraries));
    }

    [Fact]
    public void TourRequestController_Class_ShouldRequireAuthorization()
    {
        typeof(TourRequestController).Should().BeDecoratedWith<AuthorizeAttribute>();
    }

    [Fact]
    public void TourRequestController_PublicEndpoints_ShouldAllowAnonymous()
    {
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(TourRequestController), nameof(TourRequestController.GetTourRequest));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(TourRequestController), nameof(TourRequestController.GetTourRequests));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(TourRequestController), nameof(TourRequestController.GetTourRequestsByRegion));
    }

    [Fact]
    public void FeedbackController_Class_ShouldRequireAuthorization()
    {
        typeof(FeedbackController).Should().BeDecoratedWith<AuthorizeAttribute>();
    }

    [Fact]
    public void FeedbackController_PublicEndpoints_ShouldAllowAnonymous()
    {
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(FeedbackController), nameof(FeedbackController.GetUserFeedback));
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(FeedbackController), nameof(FeedbackController.GetPostFeedback));
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
        AssertEndpointHasAttribute<AllowAnonymousAttribute>(typeof(LocalizationController), nameof(LocalizationController.GetSupportedLanguages));
    }
}
