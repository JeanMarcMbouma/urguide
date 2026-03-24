using FluentAssertions;
using FluentValidation;
using UrGuide.Model.Tour;
using UrGuide.Services.Tour;

namespace UrGuide.UnitTests.Validators;

public class CreateTourRequestModelValidationTests
{
    private readonly CreateTourRequestModelValidation _validator = new();

    private static CreateTourRequestModel ValidModel() => new()
    {
        Title = "City Walking Tour",
        Description = "A guided walking tour through the historic city center.",
        PreferredDate = DateTime.UtcNow.AddDays(30),
        MaxParticipants = 10,
        MaxBudget = 100m,
        RegionId = "region-1",
        Tags = "walking,history"
    };

    [Fact]
    public void Valid_model_passes_validation()
    {
        var result = _validator.Validate(ValidModel());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_title_fails()
    {
        var model = ValidModel();
        model.Title = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Title_exceeding_200_chars_fails()
    {
        var model = ValidModel();
        model.Title = new string('A', 201);
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_description_fails()
    {
        var model = ValidModel();
        model.Description = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Past_preferred_date_fails()
    {
        var model = ValidModel();
        model.PreferredDate = DateTime.UtcNow.AddDays(-1);
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Zero_participants_fails()
    {
        var model = ValidModel();
        model.MaxParticipants = 0;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Over_50_participants_fails()
    {
        var model = ValidModel();
        model.MaxParticipants = 51;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Negative_budget_fails()
    {
        var model = ValidModel();
        model.MaxBudget = -1m;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_region_id_fails()
    {
        var model = ValidModel();
        model.RegionId = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }
}
