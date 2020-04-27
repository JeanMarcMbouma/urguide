using FluentValidation;

namespace UrGuide.Services.Catalogs
{
    class CreateImageCatalogModelValidation : AbstractValidator<Model.Catalogs.CreateImageCatalogModel>
    {
        public CreateImageCatalogModelValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Files).NotEmpty();
        }
    } 
}
