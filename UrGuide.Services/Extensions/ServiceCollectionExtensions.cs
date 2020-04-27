using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrGuide.Model;
using UrGuide.Services.Catalogs;
using UrGuide.Services.Shared;
using UrGuide.Services.Users;

namespace UrGuide.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUrGuideServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<Contracts.IUserService, UserService>();
            services.AddTransient<Contracts.ICatalogService, CatalogService>();

            // Validation

            // Shared
            services.AddTransient<IValidator<SetAttribute>, SetAttributeValidation>();
            services.AddTransient<IValidator<Model.Catalogs.ImageFileModel>, ImageFileModelValidation>();

            // Users
            services.AddTransient<IValidator<Model.Users.CreateGuideModel>, GuideValidation>();
            services.AddTransient<IValidator<Model.Users.CreateUserModel>, CreateUserValidation>();
            services.AddTransient<IValidator<Model.Users.ChangePasswordModel>, ChangePasswordValidation>();
            services.AddTransient<IValidator<Model.Users.ResetPasswordModel>, ResetPasswordValidation>();
            services.AddTransient<IValidator<Model.Users.EmailConfirmationModel>, EmailConfirmationValidation>();
            services.AddTransient<IValidator<Model.Users.PasswordResetRequestModel>, PasswordResetValidation>();

            // Catalog (Image gallery)
            services.AddTransient<IValidator<Model.Catalogs.CreateImageCatalogModel>, CreateImageCatalogModelValidation>();
            services.AddTransient<IValidator<Model.Catalogs.ImageCatalogModel>, ImageCatalogModelValidation>();
            services.AddTransient<IValidator<Model.Catalogs.UpdateImageCatalogModel>, UpdateImageCatalogModelValidation>();


            services.AddAutoMapper(typeof(UserMap));

            services.AddDbContext<Data.UrGuideContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Data")));

            return services;
        }
    }
}
