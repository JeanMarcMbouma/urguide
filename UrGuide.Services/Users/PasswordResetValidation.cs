using FluentValidation;

namespace UrGuide.Services.Users
{
    public class PasswordResetValidation : AbstractValidator<Model.Users.PasswordResetRequestModel>
    {
        public PasswordResetValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
