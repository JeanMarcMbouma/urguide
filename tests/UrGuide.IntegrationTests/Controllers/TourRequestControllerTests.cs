using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BbQ.MockLite;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Tour;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Controllers;
using UrGuide.WebApp.Models;

namespace UrGuide.IntegrationTests.Controllers;

public class TourRequestControllerTests
{
    private readonly Mock<ITourRequestService> _tourRequestServiceMock;
    private readonly TourRequestController _controller;

    public TourRequestControllerTests()
    {
        _tourRequestServiceMock = Mock.Create<ITourRequestService>();
        _controller = new TourRequestController(_tourRequestServiceMock.Object);
    }

    // --- CreateTourRequest ---

    [Fact]
    public async Task CreateTourRequest_ReturnsCreated_WhenSuccessful()
    {
        // Arrange
        var model = new CreateTourRequestModel
        {
            Title = "Explore Marrakech",
            Description = "Guided tour of the Medina",
            PreferredDate = DateTime.UtcNow.AddDays(30),
            MaxParticipants = 5,
            MaxBudget = 200m,
            Tags = "culture,food",
            RegionId = "region-1"
        };
        var created = new TourRequestModel
        {
            TourRequestId = "tr-1",
            Title = model.Title,
            Description = model.Description,
            PreferredDate = model.PreferredDate,
            MaxParticipants = model.MaxParticipants,
            MaxBudget = model.MaxBudget,
            Status = TourRequestStatus.Open
        };
        var outcome = Result.Of(created);
        _tourRequestServiceMock
            .Setup(s => s.CreateTourRequestAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.CreateTourRequest(model, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        var returned = createdResult.Value.Should().BeOfType<TourRequestModel>().Subject;
        returned.TourRequestId.Should().Be("tr-1");
        returned.Title.Should().Be("Explore Marrakech");
        createdResult.Location.Should().Be("/tour-requests/tr-1");
    }

    [Fact]
    public async Task CreateTourRequest_ReturnsBadRequest_OnServiceError()
    {
        // Arrange
        var model = new CreateTourRequestModel
        {
            Title = "Bad Request",
            Description = "Missing region",
            PreferredDate = DateTime.UtcNow.AddDays(1),
            MaxParticipants = 1,
            MaxBudget = 10m,
            RegionId = "invalid"
        };
        var outcome = Result.Of<TourRequestModel>().WithErrors("Region not found");
        _tourRequestServiceMock
            .Setup(s => s.CreateTourRequestAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.CreateTourRequest(model, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Region not found");
    }

    // --- GetTourRequest ---

    [Fact]
    public async Task GetTourRequest_ReturnsOk_WhenFound()
    {
        // Arrange
        var tourRequest = new TourRequestModel
        {
            TourRequestId = "tr-1",
            Title = "City Tour",
            Status = TourRequestStatus.Open,
            MaxBudget = 150m
        };
        var outcome = Result.Of(tourRequest);
        _tourRequestServiceMock
            .Setup(s => s.GetTourRequestByIdAsync("tr-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetTourRequest("tr-1", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<TourRequestModel>().Subject;
        returned.TourRequestId.Should().Be("tr-1");
        returned.MaxBudget.Should().Be(150m);
    }

    [Fact]
    public async Task GetTourRequest_ReturnsBadRequest_WhenNotFound()
    {
        // Arrange
        var outcome = Result.Of<TourRequestModel>().WithErrors("Tour request not found");
        _tourRequestServiceMock
            .Setup(s => s.GetTourRequestByIdAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetTourRequest("nonexistent", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // --- GetTourRequests (list all) ---

    [Fact]
    public async Task GetTourRequests_ReturnsOk_WithPagedResults()
    {
        // Arrange
        var tourItems = new List<TourRequestModel>
        {
            new() { TourRequestId = "tr-1", Title = "Tour A" },
            new() { TourRequestId = "tr-2", Title = "Tour B" }
        };
        var pagedList = PagedList.Of<TourRequestModel>(tourItems, 1);
        var pagination = new SearchParameters { PageNumber = 1 };
        var outcome = Result.Of(pagedList);
        _tourRequestServiceMock
            .Setup(s => s.GetTourRequestsAsync(It.IsAny<SearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetTourRequests(pagination, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<PagedList<TourRequestModel>>().Subject;
        returned.Items.Should().HaveCount(2);
        returned.ItemsCount.Should().Be(2);
    }

    [Fact]
    public async Task GetTourRequests_ReturnsBadRequest_OnError()
    {
        // Arrange
        var pagination = new SearchParameters { PageNumber = -1 };
        var outcome = Result.Of<PagedList<TourRequestModel>>().WithErrors("Invalid page number");
        _tourRequestServiceMock
            .Setup(s => s.GetTourRequestsAsync(It.IsAny<SearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetTourRequests(pagination, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // --- GetMyTourRequests ---

    [Fact]
    public async Task GetMyTourRequests_ReturnsOk_WithResults()
    {
        // Arrange
        var myTourItems = new List<TourRequestModel>
        {
            new() { TourRequestId = "tr-1", Title = "My Tour", RequesterId = "user-1" }
        };
        var pagedList = PagedList.Of<TourRequestModel>(myTourItems, 1);
        var pagination = new SearchParameters { PageNumber = 1 };
        var outcome = Result.Of(pagedList);
        _tourRequestServiceMock
            .Setup(s => s.GetMyTourRequestsAsync(It.IsAny<SearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetMyTourRequests(pagination, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<PagedList<TourRequestModel>>().Subject;
        returned.Items.Should().HaveCount(1);
    }

    // --- GetTourRequestsByRegion ---

    [Fact]
    public async Task GetTourRequestsByRegion_ReturnsOk_WithRegionalResults()
    {
        // Arrange
        var regionalItems = new List<TourRequestModel>
        {
            new() { TourRequestId = "tr-1", Title = "Regional Tour", RegionId = "r-med", RegionName = "Mediterranean" }
        };
        var pagedList = PagedList.Of<TourRequestModel>(regionalItems, 1);
        var pagination = new SearchParameters { PageNumber = 1 };
        var outcome = Result.Of(pagedList);
        _tourRequestServiceMock
            .Setup(s => s.GetTourRequestsByRegionAsync("r-med", It.IsAny<SearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetTourRequestsByRegion("r-med", pagination, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<PagedList<TourRequestModel>>().Subject;
        returned.Items.Should().HaveCount(1);
        returned.Items[0].RegionName.Should().Be("Mediterranean");
    }

    // --- CancelTourRequest ---

    [Fact]
    public async Task CancelTourRequest_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var outcome = Result.Of(true);
        _tourRequestServiceMock
            .Setup(s => s.CancelTourRequestAsync("tr-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.CancelTourRequest("tr-1", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(true);
    }

    [Fact]
    public async Task CancelTourRequest_ReturnsBadRequest_WhenNotFound()
    {
        // Arrange
        var outcome = Result.Of(false).WithErrors("Tour request not found");
        _tourRequestServiceMock
            .Setup(s => s.CancelTourRequestAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.CancelTourRequest("nonexistent", CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Tour request not found");
    }

    [Fact]
    public async Task CancelTourRequest_ReturnsBadRequest_WhenAlreadyCancelled()
    {
        // Arrange
        var outcome = Result.Of(false).WithErrors("Tour request is already cancelled");
        _tourRequestServiceMock
            .Setup(s => s.CancelTourRequestAsync("tr-cancelled", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.CancelTourRequest("tr-cancelled", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // --- UpdateBudget ---

    [Fact]
    public async Task UpdateBudget_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var budgetModel = new UpdateBudgetModel { NewBudget = 300m };
        var updatedRequest = new TourRequestModel
        {
            TourRequestId = "tr-1",
            Title = "Tour",
            MaxBudget = 300m,
            Status = TourRequestStatus.Open
        };
        var outcome = Result.Of(updatedRequest);
        _tourRequestServiceMock
            .Setup(s => s.UpdateBudgetAsync("tr-1", 300m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.UpdateBudget("tr-1", budgetModel, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<TourRequestModel>().Subject;
        returned.MaxBudget.Should().Be(300m);
    }

    [Fact]
    public async Task UpdateBudget_ReturnsBadRequest_OnServiceError()
    {
        // Arrange
        var budgetModel = new UpdateBudgetModel { NewBudget = -50m };
        var outcome = Result.Of<TourRequestModel>().WithErrors("Budget must be positive");
        _tourRequestServiceMock
            .Setup(s => s.UpdateBudgetAsync("tr-1", -50m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.UpdateBudget("tr-1", budgetModel, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Budget must be positive");
    }
}
