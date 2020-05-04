using FluentValidation;
using UrGuide.Model.Posts;

namespace UrGuide.Services.Posts
{
    class PostUpdateModelValidation : AbstractValidator<PostUpdateModel>
    {
        public PostUpdateModelValidation()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Text).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
