using FluentValidation;

namespace UrGuide.Services.Catalogs
{
    class ImageCatalogModelValidation : AbstractValidator<Model.Catalogs.ImageCatalogModel>
    {
        public ImageCatalogModelValidation()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(5);
            RuleFor(x => x.Files).NotEmpty();
        }
    }
}
