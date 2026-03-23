using FluentAssertions;
using UrGuide.Model.Results;

namespace UrGuide.UnitTests.Core;

public class ResultExtensionsTests
{
    [Fact]
    public void Result_Of_creates_successful_outcome()
    {
        var outcome = Result.Of("test");
        outcome.IsSuccess.Should().BeTrue();
        outcome.Value.Should().Be("test");
    }

    [Fact]
    public void Result_Empty_creates_successful_outcome()
    {
        var outcome = Result.Empty;
        outcome.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void WithErrors_adds_errors_to_outcome()
    {
        var outcome = Result.Of("test").WithErrors("error1", "error2");
        outcome.IsError.Should().BeTrue();
        outcome.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Combine_merges_errors_from_both_outcomes()
    {
        var outcome1 = Result.Of("test").WithErrors("error1");
        var outcome2 = Result.Of(42).WithErrors("error2");
        var combined = outcome1.Combine(outcome2);
        combined.IsError.Should().BeTrue();
        combined.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Combine_with_successful_other_preserves_original()
    {
        var outcome1 = Result.Of("test");
        var outcome2 = Result.Of(42);
        var combined = outcome1.Combine(outcome2);
        combined.IsSuccess.Should().BeTrue();
    }
}
