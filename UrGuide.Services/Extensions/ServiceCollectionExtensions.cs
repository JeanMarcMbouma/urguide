using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrGuide.Model;
using UrGuide.Model.Shared;
using UrGuide.Services.Catalogs;
using UrGuide.Services.Feedback;
using UrGuide.Services.Posts;
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
            services.AddTransient<Contracts.IPostService, PostService>();
            services.AddTransient<Contracts.IBidService, PostService>();
            services.AddTransient<Contracts.IFeedbackService, FeedbackService>();

            // Validation

            // Shared
            services.AddTransient<IValidator<SetAttribute>, SetAttributeValidation>();
            services.AddTransient<IValidator<ImageFileCreateModel>, ImageFileModelValidation>();

            // Users
            services.AddTransient<IValidator<Model.Users.CreateGuideModel>, GuideValidation>();
            services.AddTransient<IValidator<Model.Users.LoginModel>, LoginValidation>();
            services.AddTransient<IValidator<Model.Users.CreateUserModel>, CreateUserValidation>();
            services.AddTransient<IValidator<Model.Users.ChangePasswordModel>, ChangePasswordValidation>();
            services.AddTransient<IValidator<Model.Users.ResetPasswordModel>, ResetPasswordValidation>();
            services.AddTransient<IValidator<Model.Users.EmailConfirmationModel>, EmailConfirmationValidation>();
            services.AddTransient<IValidator<Model.Users.PasswordResetRequestModel>, PasswordResetValidation>();

            // Catalog (Image gallery)
            services.AddTransient<IValidator<Model.Catalogs.CreateImageCatalogModel>, CreateImageCatalogModelValidation>();
            services.AddTransient<IValidator<Model.Catalogs.ImageCatalogModel>, ImageCatalogModelValidation>();
            services.AddTransient<IValidator<Model.Catalogs.UpdateImageCatalogModel>, UpdateImageCatalogModelValidation>();

            // Post
            services.AddTransient<IValidator<Model.Posts.PostCreationModel>, PostCreationModelValidation>();
            services.AddTransient<IValidator<Model.Posts.PostUpdateModel>, PostUpdateModelValidation>();
            services.AddTransient<IValidator<Model.Posts.BidModel>, BidModelValidation>();

            // Feedback
            services.AddTransient<IValidator<FeedbackModel>, FeedbackModelValidator>();

            services.AddAutoMapper(typeof(UserMap));

            services.AddDbContext<Data.UrGuideContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Data"), options => options.UseNetTopologySuite()));

            return services;
        }
    }
}
