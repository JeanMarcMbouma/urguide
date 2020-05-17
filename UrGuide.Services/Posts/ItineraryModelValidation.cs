using FluentValidation;
using UrGuide.Model.Posts;

namespace UrGuide.Services.Posts
{
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
