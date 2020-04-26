using FluentValidation;

namespace UrGuide.Services.Users
{
    public class GuideValidation : AbstractValidator<Model.Users.CreateGuideModel>
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
}
