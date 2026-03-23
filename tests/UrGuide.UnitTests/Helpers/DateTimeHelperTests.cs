using FluentAssertions;
using UrGuide.Services;
using UrGuide.Services.Helpers;

namespace UrGuide.UnitTests.Helpers;

public class DateTimeHelperTests
{
    [Fact]
    public void GetDate_with_valid_date_returns_formatted_string()
    {
        DateTime? date = new DateTime(2024, 3, 15);
        var result = DateTimeHelper.GetDate(date);
        result.Should().Be("15-Mar-2024");
    }

    [Fact]
    public void GetDate_with_null_returns_unknown()
    {
        var result = DateTimeHelper.GetDate(null);
        result.Should().Be(Constants.Unknown);
    }

    [Fact]
    public void GetTime_with_valid_date_returns_formatted_time()
    {
        DateTime? date = new DateTime(2024, 3, 15, 14, 30, 0);
        var result = DateTimeHelper.GetTime(date);
        result.Should().Be("14:30");
    }

    [Fact]
    public void GetTime_with_null_returns_unknown()
    {
        var result = DateTimeHelper.GetTime(null);
        result.Should().Be(Constants.Unknown);
    }

    [Fact]
    public void GetDateTime_with_valid_date_returns_formatted_datetime()
    {
        DateTime? date = new DateTime(2024, 3, 15, 14, 30, 45);
        var result = DateTimeHelper.GetDateTime(date);
        result.Should().Be("15-Mar-2024 14:30:45");
    }

    [Fact]
    public void GetDateTime_with_null_returns_unknown()
    {
        var result = DateTimeHelper.GetDateTime(null);
        result.Should().Be(Constants.Unknown);
    }
}
