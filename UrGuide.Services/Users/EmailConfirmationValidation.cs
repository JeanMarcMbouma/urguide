using FluentValidation;

namespace UrGuide.Services.Users
{
    public class EmailConfirmationValidation : AbstractValidator<Model.Users.EmailConfirmationModel>
    {
        public EmailConfirmationValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.ConfirmationToken).NotEmpty();
        }
    }
}
