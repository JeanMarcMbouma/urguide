using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BbQ.MockLite;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Controllers;
using UrGuide.WebApp.Models;

namespace UrGuide.IntegrationTests.Controllers;

public class FeedbackControllerTests
{
    private readonly Mock<IFeedbackService> _feedbackServiceMock;
    private readonly FeedbackController _controller;

    public FeedbackControllerTests()
    {
        _feedbackServiceMock = Mock.Create<IFeedbackService>();
        _controller = new FeedbackController(_feedbackServiceMock.Object);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenServiceIsNull()
    {
        var act = () => new FeedbackController(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("feedbackService");
    }

    // --- PostFeedback tests ---

    [Fact]
    public async Task PostFeedback_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var feedback = new FeedbackModel { Text = "Great tour!", Rating = 5 };
        var outcome = Result.Of(true);
        _feedbackServiceMock
            .Setup(s => s.AddPostFeedbackAsync("post-1", feedback, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.PostFeedback("post-1", feedback, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(true);
    }

    [Fact]
    public async Task PostFeedback_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var feedback = new FeedbackModel { Text = "Bad", Rating = 1 };
        var outcome = Result.Of(false).WithErrors("Post not found");
        _feedbackServiceMock
            .Setup(s => s.AddPostFeedbackAsync("invalid-post", feedback, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.PostFeedback("invalid-post", feedback, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Post not found");
    }

    [Fact]
    public async Task PostFeedback_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var feedback = new FeedbackModel { Text = "Lovely!", Rating = 4 };
        var outcome = Result.Of(true);
        _feedbackServiceMock
            .Setup(s => s.AddPostFeedbackAsync(It.IsAny<string>(), It.IsAny<FeedbackModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        await _controller.PostFeedback("post-99", feedback, CancellationToken.None);

        // Assert
        _feedbackServiceMock.Verify(
            s => s.AddPostFeedbackAsync("post-99", feedback, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // --- UserFeedback tests ---

    [Fact]
    public async Task UserFeedback_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var feedback = new FeedbackModel { Text = "Great guide!", Rating = 5 };
        var outcome = Result.Of(true);
        _feedbackServiceMock
            .Setup(s => s.AddUserFeedbackAsync("user-1", feedback, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.UserFeedback("user-1", feedback, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(true);
    }

    [Fact]
    public async Task UserFeedback_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var feedback = new FeedbackModel { Text = "Bad guide", Rating = 1 };
        var outcome = Result.Of(false).WithErrors("User not found");
        _feedbackServiceMock
            .Setup(s => s.AddUserFeedbackAsync("bad-user", feedback, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.UserFeedback("bad-user", feedback, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // --- GetUserFeedback tests ---

    [Fact]
    public async Task GetUserFeedback_ReturnsOk_WithPagedFeedback()
    {
        // Arrange
        var feedbackItems = new List<AuthoredFeedback>
        {
            new()
            {
                Id = "fb-1",
                Text = "Amazing experience",
                Rating = 5,
                AuthorFullName = "Jane Doe",
                PublicationDate = "2024-01-15"
            }
        };
        var pagedFeedback = PagedList.Of<AuthoredFeedback>(feedbackItems, 1);
        var pagination = new PaginationParameters { PageNumber = 1 };
        var outcome = Result.Of(pagedFeedback);
        _feedbackServiceMock
            .Setup(s => s.GetUserFeedback("user-1", It.IsAny<PaginationParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetUserFeedback("user-1", pagination, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<PagedList<AuthoredFeedback>>().Subject;
        returned.Items.Should().HaveCount(1);
        returned.Items[0].Text.Should().Be("Amazing experience");
    }

    [Fact]
    public async Task GetUserFeedback_ReturnsBadRequest_OnError()
    {
        // Arrange
        var pagination = new PaginationParameters { PageNumber = 1 };
        var outcome = Result.Of<PagedList<AuthoredFeedback>>().WithErrors("User does not exist");
        _feedbackServiceMock
            .Setup(s => s.GetUserFeedback("bad-user", It.IsAny<PaginationParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetUserFeedback("bad-user", pagination, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("User does not exist");
    }

    // --- GetPostFeedback tests ---

    [Fact]
    public async Task GetPostFeedback_ReturnsOk_WithPagedFeedback()
    {
        // Arrange
        var feedbackItems = new List<AuthoredFeedback>
        {
            new() { Id = "fb-1", Text = "Nice", Rating = 4 },
            new() { Id = "fb-2", Text = "Awesome", Rating = 5 }
        };
        var pagedFeedback = PagedList.Of<AuthoredFeedback>(feedbackItems, 1);
        var pagination = new PaginationParameters { PageNumber = 1 };
        var outcome = Result.Of(pagedFeedback);
        _feedbackServiceMock
            .Setup(s => s.GetPostFeedback("post-1", It.IsAny<PaginationParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetPostFeedback("post-1", pagination, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<PagedList<AuthoredFeedback>>().Subject;
        returned.Items.Should().HaveCount(2);
    }

    // --- RespondToFeedback tests ---

    [Fact]
    public async Task RespondToFeedback_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var response = new FeedbackResponseModel { Response = "Thank you for your feedback!" };
        var outcome = Result.Of(true);
        _feedbackServiceMock
            .Setup(s => s.RespondToFeedbackAsync("fb-1", response.Response, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.RespondToFeedback("fb-1", response, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(true);
    }

    [Fact]
    public async Task RespondToFeedback_ReturnsBadRequest_WhenFeedbackNotFound()
    {
        // Arrange
        var response = new FeedbackResponseModel { Response = "Thanks!" };
        var outcome = Result.Of(false).WithErrors("Feedback not found");
        _feedbackServiceMock
            .Setup(s => s.RespondToFeedbackAsync("invalid-fb", response.Response, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.RespondToFeedback("invalid-fb", response, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Feedback not found");
    }
}
