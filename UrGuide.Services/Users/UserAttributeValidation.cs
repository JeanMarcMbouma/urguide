using FluentValidation;

namespace UrGuide.Services.Users
{
    public class UserAttributeValidation : AbstractValidator<Model.Users.SetUserAttribute>
    {
        public UserAttributeValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Value).NotEmpty();
        }
    }
}
