using Elasticsearch.Net;
using FluentValidation;
using BbQ.Cqrs.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using UrGuide.Model;
using UrGuide.Model.Shared;
using UrGuide.Services.Auditing.Command;
using UrGuide.Services.Catalogs;
using UrGuide.Services.Feedback;
using UrGuide.Services.Lookup;
using UrGuide.Services.Media;
using UrGuide.Services.Payments;
using UrGuide.Services.Posts;
using UrGuide.Services.Search;
using UrGuide.Services.Shared;
using UrGuide.Services.Tour;
using UrGuide.Services.Users;
using UrGuide.Services.Analytics;

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
            services.AddTransient<Contracts.IDataSeedingService, Seeding.DataSeedingService>();
            
            // Payment services
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IPayoutService, PayoutService>();
            services.AddTransient<IRefundService, RefundService>();
            
            // Webhook services
            services.AddTransient<Webhooks.IWebhookService, Webhooks.WebhookService>();
            services.AddHttpClient(); // Required for webhook delivery
            
            // Review moderation services
            services.AddTransient<Reviews.IReviewModerationService, Reviews.ReviewModerationService>();
            
            // Tour template services
            services.AddTransient<Templates.ITourTemplateService, Templates.TourTemplateService>();
            
            // Data export service
            services.AddTransient<Contracts.IDataExportService, DataExport.DataExportService>();
            
            // Search services
            services.AddTransient<Contracts.ISearchAnalyticsService, SearchAnalyticsService>();
            
            // Analytics service
            services.AddTransient<Contracts.IAnalyticsService, AnalyticsService>();

            // Referral services
            services.AddTransient<Referrals.IReferralService, Referrals.ReferralService>();

            // Image processing services
            services.AddTransient<IImageProcessingService, ImageProcessingService>();

            // Dispute services
            services.AddTransient<Disputes.IDisputeService, Disputes.DisputeService>();

            // Elasticsearch
            var elasticsearchUrl = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
            var elasticsearchUsername = configuration["Elasticsearch:Username"];
            var elasticsearchPassword = configuration["Elasticsearch:Password"];

            var connectionPool = new SingleNodeConnectionPool(new Uri(elasticsearchUrl));
            var connectionSettings = new ConnectionSettings(connectionPool)
                .DefaultIndex(configuration["Elasticsearch:DefaultIndex"] ?? "urguide")
                .EnableApiVersioningHeader(); // Enable compatibility with Elasticsearch 8.x

            if (!string.IsNullOrEmpty(elasticsearchUsername) && !string.IsNullOrEmpty(elasticsearchPassword))
            {
                connectionSettings = connectionSettings.BasicAuthentication(elasticsearchUsername, elasticsearchPassword);
            }

            var elasticClient = new ElasticClient(connectionSettings);
            services.AddSingleton<IElasticClient>(elasticClient);
            services.AddTransient<Contracts.IElasticsearchService, ElasticsearchService>();

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

            services.AddBbQMediator(typeof(UserDeleteAccountCommand).Assembly);

            var dataConnectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration.GetConnectionString("Data");

            services.AddDbContext<Data.UrGuideContext>(options =>
                options.UseSqlServer(
                    dataConnectionString, options => options.UseNetTopologySuite())
                .UseLazyLoadingProxies());

            return services;
        }
    }
}
