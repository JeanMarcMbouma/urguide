using FluentValidation;

namespace UrGuide.Services.Users
{
    public class UserValidation : AbstractValidator<Model.Users.CreateUserCommand>
    {
        public UserValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.ConfirmPassword).NotEmpty().MinimumLength(8).Matches(x => x.Password);
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(4);
        }
    }

    public class GuideValidation : AbstractValidator<Model.Users.CreateGuideCommand>
    {
        public GuideValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.ConfirmPassword).NotEmpty().MinimumLength(8).Matches(x => x.Password);
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.Phone).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
            RuleFor(x => x.BirthDay).NotEmpty();
        }
    }

    public class UserAttributeValidation : AbstractValidator<Model.Users.SetUserAttribute>
    {
        public UserAttributeValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Value).NotEmpty();
        }
    }
    public class LoginValidation : AbstractValidator<Model.Users.LoginCommand>
    {
        public LoginValidation()
        {
            RuleFor(x => x.UserName).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}
