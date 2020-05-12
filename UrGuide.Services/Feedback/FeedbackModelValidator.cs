using FluentValidation;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Feedback
{
    class FeedbackModelValidator : AbstractValidator<FeedbackModel>
    {
        public FeedbackModelValidator()
        {
            RuleFor(x => x.Text).MinimumLength(50).MaximumLength(500);
            RuleFor(x => x.Rating).Must(x => x <= 5 || x >= 0).WithMessage("Rating shall be between 0 and 5");
        }
    }
}
