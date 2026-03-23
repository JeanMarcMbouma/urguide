using FluentAssertions;
using FluentValidation;
using UrGuide.Model;
using UrGuide.Services.Shared;

namespace UrGuide.UnitTests.Validators;

public class SetAttributeValidationTests
{
    private readonly SetAttributeValidation _validator = new();

    private static SetAttribute ValidModel() => new()
    {
        Name = "Language",
        Value = "English"
    };

    [Fact]
    public void Valid_model_passes_validation()
    {
        var result = _validator.Validate(ValidModel());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_name_fails()
    {
        var model = ValidModel();
        model.Name = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_value_fails()
    {
        var model = ValidModel();
        model.Value = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }
}
