using FluentValidation;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Users
{
    public class UpdateUserValidation : AbstractValidator<Model.Users.UpdateUserModel>
    {
        public UpdateUserValidation(IUserContext userContext)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Id).Must(x => x.Equals(userContext.UserId)).WithMessage(ErrorMessages.NotAuthorized);
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.ProfileImage).NotEmpty();
        }

    }
}
