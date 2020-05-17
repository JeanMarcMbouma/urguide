using FluentValidation;
using UrGuide.Model.Posts;

namespace UrGuide.Services.Posts
{
    class UserReactionModelValidator : AbstractValidator<UserReactionModel>
    {
        public UserReactionModelValidator()
        {
            RuleFor(x => x.PostId).NotEmpty();
        }
    }
}
