using FluentValidation;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Shared
{
    class ImageFileModelValidation : AbstractValidator<ImageFileModel>
    {
        public ImageFileModelValidation()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(5);
            RuleFor(x => x.ImageBase64).NotEmpty();
        }
    }
}
