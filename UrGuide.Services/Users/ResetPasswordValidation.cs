using FluentValidation;

namespace UrGuide.Services.Users
{
    public class ResetPasswordValidation : AbstractValidator<Model.Users.ResetPasswordModel>
    {
        public ResetPasswordValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.ConfirmPassword).NotEmpty().Matches(x => x.Password);
            RuleFor(x => x.ConfirmationToken).NotEmpty();
        }
    }
}
