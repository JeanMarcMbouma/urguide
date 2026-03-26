using FluentValidation;
using UrGuide.Model.PushNotifications;

namespace UrGuide.Services.PushNotifications;

class DeviceRegistrationValidator : AbstractValidator<DeviceRegistrationRequest>
{
    public DeviceRegistrationValidator()
    {
        RuleFor(x => x.DeviceToken).NotEmpty();
        RuleFor(x => x.Platform).IsInEnum();
        RuleFor(x => x.DeviceName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.AppVersion).NotEmpty().MaximumLength(64);
    }
}
