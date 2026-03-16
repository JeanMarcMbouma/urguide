using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Entities;
using Duende.IdentityModel;

namespace UrGuide.WebApp.Services
{
    public class ProfileService : IProfileService
    {
        public ProfileService(IUserService userService, UserManager<UrGuideUser> userManager, IUserClaimsPrincipalFactory<UrGuideUser> principalFactory)
        {
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            PrincipalFactory = principalFactory ?? throw new ArgumentNullException(nameof(principalFactory));
        }

        public IUserService UserService { get; }
        public UserManager<UrGuideUser> UserManager { get; }
        public IUserClaimsPrincipalFactory<UrGuideUser> PrincipalFactory { get; }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var result = await UserService.GetUserAsync(context.Subject.GetSubjectId(), CancellationToken.None);
            var principal = await UserManager.FindByIdAsync(context.Subject.GetSubjectId());
            if (result.IsError || result.Value == null || principal == null)
            {
                return;
            }

            var claimPrincipal = await PrincipalFactory.CreateAsync(principal);

            context.IssuedClaims.AddRange(claimPrincipal.Claims);
            SafeAddClaims(context, JwtClaimTypes.BirthDate, result.Value.BirthDay)
                .SafeAddClaims(context, JwtClaimTypes.Picture, result.Value.ProfileImage)
                .SafeAddClaims(context, JwtClaimTypes.FamilyName, result.Value.LastName)
                .SafeAddClaims(context, JwtClaimTypes.GivenName, result.Value.FirstName)
                .SafeAddClaims(context, JwtClaimTypes.Gender, result.Value.Gender)
                .SafeAddClaims(context, JwtClaimTypes.Name, result.Value.FullName)
                .SafeAddClaims(context, JwtClaimTypes.Address, result.Value.Address)
                .SafeAddClaims(context, "country", result.Value.Country)
                .SafeAddClaims(context, JwtClaimTypes.Role, result.Value.IsGuide ? "guide" : "user");

            // Add ASP.NET Identity roles (Admin, etc.) to the token
            var roles = await UserManager.GetRolesAsync(principal);
            foreach (var role in roles)
            {
                context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, role));
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var exists = await UserService.ExistsAsync(context.Subject.GetSubjectId(), CancellationToken.None);
            context.IsActive = exists.Value;
        }

        private ProfileService SafeAddClaims(ProfileDataRequestContext context, string name, string value) {
            if(!string.IsNullOrEmpty(value))
            context.IssuedClaims.Add(new Claim(name, value));
            return this;
        }
    }
}
