using FluentValidation;

namespace UrGuide.Services.Users
{
    public class CreateUserValidation : AbstractValidator<Model.Users.CreateUserModel>
    {
        public CreateUserValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.ConfirmPassword).NotEmpty().MinimumLength(8).Matches(x => x.Password);
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(4);
        }
    }
}
