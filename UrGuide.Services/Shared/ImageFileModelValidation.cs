using FluentValidation;

namespace UrGuide.Services.Shared
{
    class ImageFileModelValidation : AbstractValidator<Model.Catalogs.ImageFileModel>
    {
        public ImageFileModelValidation()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(5);
            RuleFor(x => x.ImageBase64).NotEmpty();
        }
    }
}
