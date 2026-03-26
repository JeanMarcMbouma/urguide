using FluentAssertions;
using FluentValidation;
using UrGuide.Model.PushNotifications;
using UrGuide.Services.PushNotifications;

namespace UrGuide.UnitTests.Validators;

public class DeviceRegistrationValidationTests
{
    private readonly DeviceRegistrationValidator _validator = new();

    private static DeviceRegistrationRequest ValidModel() => new()
    {
        DeviceToken = "abc123-device-token",
        Platform = DevicePlatform.Android,
        DeviceName = "Pixel 8",
        AppVersion = "1.0.0"
    };

    [Fact]
    public void Valid_model_passes_validation()
    {
        var result = _validator.Validate(ValidModel());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_device_token_fails()
    {
        var model = ValidModel();
        model.DeviceToken = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Null_device_token_fails()
    {
        var model = ValidModel();
        model.DeviceToken = null!;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_device_name_fails()
    {
        var model = ValidModel();
        model.DeviceName = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Null_device_name_fails()
    {
        var model = ValidModel();
        model.DeviceName = null!;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Device_name_too_long_fails()
    {
        var model = ValidModel();
        model.DeviceName = new string('A', 257);
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Device_name_at_max_length_passes()
    {
        var model = ValidModel();
        model.DeviceName = new string('A', 256);
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_app_version_fails()
    {
        var model = ValidModel();
        model.AppVersion = "";
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Null_app_version_fails()
    {
        var model = ValidModel();
        model.AppVersion = null!;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void App_version_too_long_fails()
    {
        var model = ValidModel();
        model.AppVersion = new string('1', 65);
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void App_version_at_max_length_passes()
    {
        var model = ValidModel();
        model.AppVersion = new string('1', 64);
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Invalid_platform_enum_value_fails()
    {
        var model = ValidModel();
        model.Platform = (DevicePlatform)999;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(DevicePlatform.iOS)]
    [InlineData(DevicePlatform.Android)]
    [InlineData(DevicePlatform.Web)]
    public void Valid_platform_values_pass(DevicePlatform platform)
    {
        var model = ValidModel();
        model.Platform = platform;
        var result = _validator.Validate(model);
        result.IsValid.Should().BeTrue();
    }
}
