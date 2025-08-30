using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrGuide.Model;
using UrGuide.Model.Shared;
using UrGuide.Services.Auditing.Command;
using UrGuide.Services.Catalogs;
using UrGuide.Services.Feedback;
using UrGuide.Services.Lookup;
using UrGuide.Services.Media;
using UrGuide.Services.Posts;
using UrGuide.Services.Shared;
using UrGuide.Services.Tour;
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
            services.AddTransient<Contracts.IImageService, ImageService>();
            services.AddTransient<Contracts.ILookupService, LookupService>();
            services.AddTransient<Contracts.IUserNotificationService, NotificationService>();
            services.AddTransient<Contracts.ITourRequestService, TourRequestService>();

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
            services.AddTransient<IValidator<Model.Users.CreateNotification>, CreateNotificationValidator>();
            services.AddTransient<IValidator<Model.Messages.ChatMessage>, ChatMessageValidator>();
            services.AddTransient<IValidator<Model.Users.UpdateUserModel>, UpdateUserValidation>();
            services.AddTransient<IValidator<Model.Users.UpdateGuideModel>, UpdateGuideValidation>();

            // Catalog (Image gallery)
            services.AddTransient<IValidator<Model.Catalogs.CreateImageCatalogModel>, CreateImageCatalogModelValidation>();
            services.AddTransient<IValidator<Model.Catalogs.ImageCatalogModel>, ImageCatalogModelValidation>();
            services.AddTransient<IValidator<Model.Catalogs.UpdateImageCatalogModel>, UpdateImageCatalogModelValidation>();

            // Post
            services.AddTransient<IValidator<Model.Posts.PostCreationModel>, PostCreationModelValidation>();
            services.AddTransient<IValidator<Model.Posts.PostUpdateModel>, PostUpdateModelValidation>();
            services.AddTransient<IValidator<Model.Posts.BidModel>, BidModelValidation>();
            services.AddTransient<IValidator<Model.Posts.UserReactionModel>, UserReactionModelValidator>();
            services.AddTransient<IValidator<Model.Posts.SeatReservationModel>, SeatReservationModelValidator>();
            services.AddTransient<IValidator<SearchParameters>, SearchParametersValidator>();
            services.AddTransient<IValidator<PaginationParameters>, PaginationParameterValidator>();

            // Tour Requests
            services.AddTransient<IValidator<Model.Tour.CreateTourRequestModel>, CreateTourRequestModelValidation>();

            // Feedback
            services.AddTransient<IValidator<FeedbackModel>, FeedbackModelValidator>();

            services.AddAutoMapper(typeof(UserMap));

            services.AddMediatR(typeof(UserDeleteAccountCommand).Assembly);

            services.AddDbContext<Data.UrGuideContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Data"), options => options.UseNetTopologySuite())
                .UseLazyLoadingProxies());

            return services;
        }
    }
}
