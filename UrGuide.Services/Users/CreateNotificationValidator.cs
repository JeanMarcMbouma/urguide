using FluentValidation;
using UrGuide.Model.Users;

namespace UrGuide.Services.Users
{
    class CreateNotificationValidator : AbstractValidator<CreateNotification>
    {
        public CreateNotificationValidator()
        {
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.IsSystem).Must((x, v) => !x.IsSystem || x.AuthorId == Constants.SystemUserId);
            RuleFor(x => x.AuthorId).NotEmpty();
        }
    }
}
