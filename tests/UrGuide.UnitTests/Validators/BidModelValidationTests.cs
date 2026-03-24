using FluentAssertions;
using FluentValidation;
using UrGuide.Model.Posts;
using UrGuide.Services.Posts;

namespace UrGuide.UnitTests.Validators;

public class BidModelValidationTests
{
    private readonly BidModelValidation _validator = new();

    private static BidModel ValidModel() => new()
    {
        PostId = "post-123",
        Value = "50.00"
    };

    [Fact]
    public void Valid_model_passes_validation()
    {
        var result = _validator.Validate(ValidModel());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_PostId_fails()
    {
        var model = ValidModel();
        model.PostId = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_Value_fails()
    {
        var model = ValidModel();
        model.Value = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }
}
