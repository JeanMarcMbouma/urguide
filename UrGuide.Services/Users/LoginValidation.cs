using FluentValidation;

namespace UrGuide.Services.Users
{
    public class LoginValidation : AbstractValidator<Model.Users.LoginModel>
    {
        public LoginValidation()
        {
            RuleFor(x => x.UserName).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}
