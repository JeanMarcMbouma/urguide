using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrGuide.Services.Users;

namespace UrGuide.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUrGuideServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<Contracts.IUserService, UserService>();
            services.AddTransient<IValidator<Model.Users.CreateGuideCommand>, GuideValidation>();
            services.AddTransient<IValidator<Model.Users.CreateUserCommand>, UserValidation>();
            services.AddTransient<IValidator<Model.Users.SetUserAttribute>, UserAttributeValidation>();
            services.AddAutoMapper(typeof(UserMap));

            services.AddDbContext<Data.UrGuideContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Data")));

            return services;
        }
    }
}
