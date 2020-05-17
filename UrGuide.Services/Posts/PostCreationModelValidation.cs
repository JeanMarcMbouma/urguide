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
            RuleFor(x => x.Images).Must(f => f.Count <= Constants.MaxImageCountPerPost).WithMessage($"You cannot upload more than {Constants.MaxImageCountPerPost} images");
        }
    }
}
