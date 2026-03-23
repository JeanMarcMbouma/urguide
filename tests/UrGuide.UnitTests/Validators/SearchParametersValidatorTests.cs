using FluentAssertions;
using FluentValidation;
using UrGuide.Model;
using UrGuide.Services.Posts;

namespace UrGuide.UnitTests.Validators;

public class SearchParametersValidatorTests
{
    private readonly SearchParametersValidator _validator = new();

    [Fact]
    public void Valid_search_params_passes()
    {
        var model = new SearchParameters { PageNumber = 1 };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void PageNumber_of_zero_fails()
    {
        var model = new SearchParameters { PageNumber = 0 };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Negative_page_number_fails()
    {
        var model = new SearchParameters { PageNumber = -5 };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void PageNumber_of_1_passes()
    {
        var model = new SearchParameters { PageNumber = 1 };
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }
}
