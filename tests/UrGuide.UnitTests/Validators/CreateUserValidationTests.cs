using FluentAssertions;
using FluentValidation;
using UrGuide.Model.Users;
using UrGuide.Services.Users;

namespace UrGuide.UnitTests.Validators;

public class CreateUserValidationTests
{
    private readonly CreateUserValidation _validator = new();

    private static CreateUserModel ValidModel() => new()
    {
        Email = "user@example.com",
        Password = "Password123",
        ConfirmPassword = "Password123",
        FirstName = "John",
        LastName = "Doe Smith"
    };

    [Fact]
    public void Valid_model_passes_validation()
    {
        var result = _validator.Validate(ValidModel());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_email_fails()
    {
        var model = ValidModel();
        model.Email = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Invalid_email_fails()
    {
        var model = ValidModel();
        model.Email = "not-an-email";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_password_fails()
    {
        var model = ValidModel();
        model.Password = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Short_password_fails()
    {
        var model = ValidModel();
        model.Password = "short";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ConfirmPassword_not_matching_fails()
    {
        var model = ValidModel();
        model.ConfirmPassword = "Different1";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Short_first_name_fails()
    {
        var model = ValidModel();
        model.FirstName = "Jo";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Short_last_name_fails()
    {
        var model = ValidModel();
        model.LastName = "Do";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }
}
