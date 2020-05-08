using FluentValidation;
using UrGuide.Model.Posts;

namespace UrGuide.Services.Posts
{
    class BidModelValidation : AbstractValidator<BidModel>
    {
        public BidModelValidation()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Value).NotEmpty();
        }
    }
}
