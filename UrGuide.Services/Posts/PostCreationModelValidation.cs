using FluentValidation;
using UrGuide.Model.Posts;

namespace UrGuide.Services.Posts
{
    class PostCreationModelValidation : AbstractValidator<PostCreationModel>
    {
        public PostCreationModelValidation()
        {
            RuleFor(x => x.Categories).NotEmpty();
            RuleFor(x => x.Text).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Images).Must(f => f.Count <= 3).WithMessage("You cannot upload more than three images");
        }
    }

    class ItineraryModelValidation : AbstractValidator<ItineraryModel>
    {
        public ItineraryModelValidation()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Ordinal).GreaterThan(0);
        }
    }
}
