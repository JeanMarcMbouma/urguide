using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BbQ.MockLite;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Lookup;
using UrGuide.Model.Results;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Controllers;
using UrGuide.WebApp.Models;

namespace UrGuide.IntegrationTests.Controllers;

public class LookupControllerTests
{
    private readonly Mock<ILookupService> _lookupServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly LookupController _controller;

    public LookupControllerTests()
    {
        _lookupServiceMock = Mock.Create<ILookupService>();
        _userServiceMock = Mock.Create<IUserService>();
        _controller = new LookupController(_lookupServiceMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task GetCategories_ReturnsOk_WithCategoryList()
    {
        // Arrange
        var categories = new List<CategoryModel>
        {
            new() { Name = "Historical", ImageUrl = "http://example.com/hist.jpg", Stats = 10 },
            new() { Name = "Adventure", ImageUrl = "http://example.com/adv.jpg", Stats = 25 }
        };
        var outcome = Result.Of<IEnumerable<CategoryModel>>(categories);
        _lookupServiceMock
            .Setup(s => s.GetCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetCategories(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryModel>>().Subject;
        returnedCategories.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCategories_ReturnsOk_WithEmptyList()
    {
        // Arrange
        var outcome = Result.Of<IEnumerable<CategoryModel>>(Enumerable.Empty<CategoryModel>());
        _lookupServiceMock
            .Setup(s => s.GetCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetCategories(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryModel>>().Subject;
        returnedCategories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRegions_ReturnsOk_WithRegionList()
    {
        // Arrange
        var regions = new List<RegionModel>
        {
            new() { RegionId = "r1", Name = "Mediterranean", CurrencyId = "EUR" },
            new() { RegionId = "r2", Name = "Caribbean", CurrencyId = "USD" }
        };
        var outcome = Result.Of<IEnumerable<RegionModel>>(regions);
        _lookupServiceMock
            .Setup(s => s.GetRegionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetRegions(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRegions = okResult.Value.Should().BeAssignableTo<IEnumerable<RegionModel>>().Subject;
        returnedRegions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetOne_ReturnsOk_WhenUserFound()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Id = "user-1",
            FullName = "John Doe",
            FirstName = "John",
            LastName = "Doe",
            Country = "USA",
            City = "New York",
            Rating = 5
        };
        var outcome = Result.Of(userInfo);
        _userServiceMock
            .Setup(s => s.GetUserInfo("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetOne("user-1", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUser = okResult.Value.Should().BeOfType<UserInfo>().Subject;
        returnedUser.Id.Should().Be("user-1");
        returnedUser.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetOne_ReturnsBadRequest_WhenUserNotFound()
    {
        // Arrange
        var outcome = Result.Of<UserInfo>().WithErrors("User not found");
        _userServiceMock
            .Setup(s => s.GetUserInfo("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetOne("nonexistent", CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task GetUsers_ReturnsOk_WithPagedResults()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new() { Id = "u1", FullName = "Alice" },
            new() { Id = "u2", FullName = "Bob" }
        };
        var pagedUsers = PagedList.Of<UserInfo>(users, 1);
        var searchParams = new SearchParameters { PageNumber = 1, Term = "test" };
        var outcome = Result.Of(pagedUsers);
        _userServiceMock
            .Setup(s => s.GetUsersAsync(It.IsAny<SearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetUsers(searchParams, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedList = okResult.Value.Should().BeOfType<PagedList<UserInfo>>().Subject;
        returnedList.Items.Should().HaveCount(2);
        returnedList.ItemsCount.Should().Be(2);
    }

    [Fact]
    public async Task GetUsers_ReturnsBadRequest_WhenServiceErrors()
    {
        // Arrange
        var searchParams = new SearchParameters { PageNumber = 1 };
        var outcome = Result.Of<PagedList<UserInfo>>().WithErrors("Invalid search parameters");
        _userServiceMock
            .Setup(s => s.GetUsersAsync(It.IsAny<SearchParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var result = await _controller.GetUsers(searchParams, CancellationToken.None);

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorEnvelop = badResult.Value.Should().BeOfType<ErrorEnvelop<string>>().Subject;
        errorEnvelop.Errors.Should().Contain("Invalid search parameters");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLookupServiceIsNull()
    {
        var act = () => new LookupController(null!, _userServiceMock.Object);
        act.Should().Throw<ArgumentNullException>().WithParameterName("lookupService");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenUserServiceIsNull()
    {
        var act = () => new LookupController(_lookupServiceMock.Object, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("userService");
    }
}
