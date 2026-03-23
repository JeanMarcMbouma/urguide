using FluentAssertions;
using FluentValidation;
using UrGuide.Model.Posts;
using UrGuide.Model.Shared;
using UrGuide.Services;
using UrGuide.Services.Posts;

namespace UrGuide.UnitTests.Validators;

public class PostCreationModelValidationTests
{
    private readonly PostCreationModelValidation _validator = new();

    private static PostCreationModel ValidModel() => new()
    {
        Text = "Amazing tour through the city",
        Description = "A detailed description of the tour experience",
        Categories = new HashSet<string> { "walking", "history" }
    };

    [Fact]
    public void Valid_model_passes_validation()
    {
        var result = _validator.Validate(ValidModel());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_categories_fails()
    {
        var model = ValidModel();
        model.Categories = new HashSet<string>();
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_text_fails()
    {
        var model = ValidModel();
        model.Text = "";
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
    public void Too_many_images_fails()
    {
        var model = ValidModel();
        var images = new HashSet<ImageFileCreateModel>();
        for (int i = 0; i < Constants.MaxImageCountPerPost + 1; i++)
        {
            images.Add(new ImageFileCreateModel { Name = $"image{i}.jpg" });
        }
        model.Images = images;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }
}
