using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Controllers;
using UrGuide.WebApp.Models;

namespace UrGuide.IntegrationTests.Controllers;

public class PostControllerTests
{
    private readonly Mock<IPostService> _postServiceMock;
    private readonly PostController _controller;

    public PostControllerTests()
    {
        _postServiceMock = new Mock<IPostService>();
        _controller = new PostController(_postServiceMock.Object);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenServiceIsNull()
    {
        var act = () => new PostController(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("postService");
    }

    // --- GetLast10 ---

    [Fact]
    public async Task Get_ReturnsOk_WithLast10Posts()
    {
        // Arrange
        var posts = new List<PostModel>
        {
            new() { Id = "p1", Text = "Tour of Rome" },
            new() { Id = "p2", Text = "Paris Adventure" }
        };
        var outcome = Result.Of<IEnumerable<PostModel>>(posts);
        _postServiceMock
            .Setup(s => s.GetLast10PostsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Get(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeAssignableTo<IEnumerable<PostModel>>().Subject;
        returned.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_OnServiceError()
    {
        // Arrange
        var outcome = Result.Of<IEnumerable<PostModel>>().WithErrors("Database unavailable");
        _postServiceMock
            .Setup(s => s.GetLast10PostsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Get(CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // --- GetLast100 ---

    [Fact]
    public async Task Last100_ReturnsOk_WithPosts()
    {
        // Arrange
        var posts = new List<PostModel> { new() { Id = "p1", Text = "Some tour" } };
        var outcome = Result.Of<IEnumerable<PostModel>>(posts);
        _postServiceMock
            .Setup(s => s.GetLast100PostsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Last100(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // --- Top10 ---

    [Fact]
    public async Task Top10_ReturnsOk_WithPosts()
    {
        // Arrange
        var posts = new List<PostModel>
        {
            new() { Id = "p1", Text = "Best Tour", Likes = 100 }
        };
        var outcome = Result.Of<IEnumerable<PostModel>>(posts);
        _postServiceMock
            .Setup(s => s.GetTop10PostsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Top10(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeAssignableTo<IEnumerable<PostModel>>().Subject;
        returned.Should().HaveCount(1);
    }

    // --- Top100 ---

    [Fact]
    public async Task Top100_ReturnsBadRequest_OnServiceError()
    {
        // Arrange
        var outcome = Result.Of<IEnumerable<PostModel>>().WithErrors("Service error");
        _postServiceMock
            .Setup(s => s.GetTop100PostsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Top100(CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Service error");
    }

    // --- GetOne ---

    [Fact]
    public async Task GetOne_ReturnsOk_WhenPostFound()
    {
        // Arrange
        var post = new PostModel { Id = "post-1", Text = "Great Tour", Description = "Visit landmarks" };
        var outcome = Result.Of(post);
        _postServiceMock
            .Setup(s => s.GetByIdAsync("post-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetOne("post-1", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<PostModel>().Subject;
        returned.Id.Should().Be("post-1");
        returned.Text.Should().Be("Great Tour");
    }

    [Fact]
    public async Task GetOne_ReturnsBadRequest_WhenPostNotFound()
    {
        // Arrange
        var outcome = Result.Of<PostModel>().WithErrors("Post not found");
        _postServiceMock
            .Setup(s => s.GetByIdAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetOne("nonexistent", CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Post not found");
    }

    // --- SearchPost ---

    [Fact]
    public async Task SearchPost_ReturnsOk_WithPagedResults()
    {
        // Arrange
        var postItems = new List<PostModel> { new() { Id = "p1", Text = "Matching tour" } };
        var pagedPosts = PagedList.Of<PostModel>(postItems, 1);
        var pagination = new SearchParameters { PageNumber = 1, Term = "tour" };
        var outcome = Result.Of(pagedPosts);
        _postServiceMock
            .Setup(s => s.GetPostsAsync(It.IsAny<SearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.SearchPost(pagination, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<PagedList<PostModel>>().Subject;
        returned.Items.Should().HaveCount(1);
    }

    // --- Create ---

    [Fact]
    public async Task Create_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var model = new PostCreationModel
        {
            Text = "New Tour",
            Description = "Explore the city",
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(8),
            Seats = 10,
            UnitPrice = "50"
        };
        var createdPost = new PostModel { Id = "new-post-1", Text = "New Tour", Description = "Explore the city" };
        var outcome = Result.Of(createdPost);
        _postServiceMock
            .Setup(s => s.CreatePostAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Create(model, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<PostModel>().Subject;
        returned.Id.Should().Be("new-post-1");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_OnValidationError()
    {
        // Arrange
        var model = new PostCreationModel { Text = "" };
        var outcome = Result.Of<PostModel>().WithErrors("Text is required");
        _postServiceMock
            .Setup(s => s.CreatePostAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Create(model, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Text is required");
    }

    // --- Edit ---

    [Fact]
    public async Task Edit_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var model = new PostUpdateModel { Id = "post-1", Text = "Updated", Description = "Updated desc" };
        var outcome = Result.Of(true);
        _postServiceMock
            .Setup(s => s.UpdatePostAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Edit("post-1", model, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Edit_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var model = new PostUpdateModel { Id = "post-2", Text = "Updated" };

        // Act
        var result = await _controller.Edit("post-1", model, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Edit_ReturnsBadRequest_OnServiceError()
    {
        // Arrange
        var model = new PostUpdateModel { Id = "post-1", Text = "Updated" };
        var outcome = Result.Of(false).WithErrors("Post not found");
        _postServiceMock
            .Setup(s => s.UpdatePostAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Edit("post-1", model, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // --- DeletePost ---

    [Fact]
    public async Task DeletePost_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var outcome = Result.Of(true);
        _postServiceMock
            .Setup(s => s.DeletePostAsync("post-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.DeletePost("post-1", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(true);
    }

    [Fact]
    public async Task DeletePost_ReturnsBadRequest_WhenNotFound()
    {
        // Arrange
        var outcome = Result.Of(false).WithErrors("Post not found");
        _postServiceMock
            .Setup(s => s.DeletePostAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.DeletePost("nonexistent", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // --- GetItineraries ---

    [Fact]
    public async Task GetItineraries_ReturnsOk_WithItineraryList()
    {
        // Arrange
        var itineraries = new List<ItineraryModel>
        {
            new() { Title = "Day 1", Description = "Visit museum", Ordinal = 1 },
            new() { Title = "Day 2", Description = "City walk", Ordinal = 2 }
        };
        var outcome = Result.Of<IEnumerable<ItineraryModel>>(itineraries);
        _postServiceMock
            .Setup(s => s.GetItinerariesAsync("post-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetItineraries("post-1", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeAssignableTo<IEnumerable<ItineraryModel>>().Subject;
        returned.Should().HaveCount(2);
    }

    // --- Reserve ---

    [Fact]
    public async Task Reserve_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var reservation = new SeatReservationModel { PostId = "post-1", Seats = 2 };
        var outcome = Result.Of(true);
        _postServiceMock
            .Setup(s => s.ReserveSeatsAsync(reservation, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Reserve("post-1", reservation, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Reserve_ReturnsBadRequest_WhenPostIdMismatch()
    {
        // Arrange
        var reservation = new SeatReservationModel { PostId = "post-2", Seats = 2 };

        // Act
        var result = await _controller.Reserve("post-1", reservation, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Reserve_ReturnsBadRequest_OnServiceError()
    {
        // Arrange
        var reservation = new SeatReservationModel { PostId = "post-1", Seats = 999 };
        var outcome = Result.Of(false).WithErrors("Not enough seats available");
        _postServiceMock
            .Setup(s => s.ReserveSeatsAsync(reservation, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.Reserve("post-1", reservation, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Not enough seats available");
    }

    // --- EditReservation ---

    [Fact]
    public async Task EditReservation_ReturnsBadRequest_WhenPostIdMismatch()
    {
        // Arrange
        var reservation = new SeatReservationModel { PostId = "post-2", Seats = 3 };

        // Act
        var result = await _controller.EditReservation("post-1", reservation, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    // --- CancelReservation ---

    [Fact]
    public async Task CancelReservation_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var outcome = Result.Of(true);
        _postServiceMock
            .Setup(s => s.CancelReservationAsync("post-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.CancelReservation("post-1", CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // --- RecordUserReaction ---

    [Fact]
    public async Task RecordUserReaction_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var reaction = new UserReactionModel { PostId = "post-1", Like = true };
        var outcome = Result.Of(true);
        _postServiceMock
            .Setup(s => s.RecordUserReactionAsync(reaction, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.RecordUserReaction("post-1", reaction, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task RecordUserReaction_ReturnsBadRequest_WhenPostIdMismatch()
    {
        // Arrange
        var reaction = new UserReactionModel { PostId = "post-2", Like = true };

        // Act
        var result = await _controller.RecordUserReaction("post-1", reaction, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }
}
