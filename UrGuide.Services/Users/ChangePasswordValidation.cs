using FluentValidation;

namespace UrGuide.Services.Users
{
    public class ChangePasswordValidation : AbstractValidator<Model.Users.ChangePasswordModel>
    {
        public ChangePasswordValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.CurrentPassword).NotEmpty().MinimumLength(8);
            RuleFor(x => x.ConfirmPassword).NotEmpty().Matches(x => x.Password);
        }
    }
}
