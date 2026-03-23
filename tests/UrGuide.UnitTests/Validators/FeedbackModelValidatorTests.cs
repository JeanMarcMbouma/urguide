using FluentAssertions;
using FluentValidation;
using UrGuide.Model.Shared;
using UrGuide.Services.Feedback;

namespace UrGuide.UnitTests.Validators;

public class FeedbackModelValidatorTests
{
    private readonly FeedbackModelValidator _validator = new();

    private static FeedbackModel ValidModel() => new()
    {
        Text = new string('A', 100),
        Rating = 4
    };

    [Fact]
    public void Valid_model_passes_validation()
    {
        var result = _validator.Validate(ValidModel());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Text_too_short_fails()
    {
        var model = ValidModel();
        model.Text = "Short text";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Text_too_long_fails()
    {
        var model = ValidModel();
        model.Text = new string('A', 501);
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Null_text_passes()
    {
        var model = ValidModel();
        model.Text = null!;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(100)]
    public void Rating_of_any_value_passes_due_to_or_bug(int rating)
    {
        var model = ValidModel();
        model.Rating = rating;
        var result = _validator.Validate(model);
        // The validator uses OR (x <= 5 || x >= 0) so any int passes
        result.IsValid.Should().BeTrue();
    }
}
