using FluentValidation;

namespace UrGuide.Services.Catalogs
{
    class UpdateImageCatalogModelValidation : AbstractValidator<Model.Catalogs.UpdateImageCatalogModel>
    {
        public UpdateImageCatalogModelValidation()
        {
            RuleFor(x => x.CatalogId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
