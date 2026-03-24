using FluentAssertions;
using FluentValidation;
using UrGuide.Model.Users;
using UrGuide.Services.Users;

namespace UrGuide.UnitTests.Validators;

public class LoginValidationTests
{
    private readonly LoginValidation _validator = new();

    private static LoginModel ValidModel() => new()
    {
        UserName = "user@example.com",
        Password = "Password123"
    };

    [Fact]
    public void Valid_model_passes_validation()
    {
        var result = _validator.Validate(ValidModel());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_username_fails()
    {
        var model = ValidModel();
        model.UserName = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Invalid_email_fails()
    {
        var model = ValidModel();
        model.UserName = "not-an-email";
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
    public void Password_less_than_8_chars_fails()
    {
        var model = ValidModel();
        model.Password = "short";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }
}
