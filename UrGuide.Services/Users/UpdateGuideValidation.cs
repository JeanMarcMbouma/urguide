using FluentValidation;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Users
{
    public class UpdateGuideValidation : AbstractValidator<Model.Users.UpdateGuideModel>
    {
        public UpdateGuideValidation(IUserContext userContext)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Id).Must(x => x.Equals(userContext.UserId)).WithMessage(ErrorMessages.NotAuthorized);
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.Phone).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
            RuleFor(x => x.BirthDay).NotEmpty();
            RuleFor(x => x.ProfileImage).NotEmpty();
            RuleFor(x => x.Description).NotEmpty().MinimumLength(100).MaximumLength(500);
        }

    }
}
