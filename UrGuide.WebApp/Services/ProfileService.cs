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
            var claimPrincipal = await PrincipalFactory.CreateAsync(principal);

            context.IssuedClaims.AddRange(claimPrincipal.Claims);
            SafeAddClaims(context, IdentityModel.JwtClaimTypes.BirthDate, result.Data.BirthDay)
                .SafeAddClaims(context, IdentityModel.JwtClaimTypes.Picture, result.Data.ProfileImage)
                .SafeAddClaims(context, IdentityModel.JwtClaimTypes.FamilyName, result.Data.LastName)
                .SafeAddClaims(context, IdentityModel.JwtClaimTypes.GivenName, result.Data.FirstName)
                .SafeAddClaims(context, IdentityModel.JwtClaimTypes.Gender, result.Data.Gender)
                .SafeAddClaims(context, IdentityModel.JwtClaimTypes.Name, result.Data.FullName)
                .SafeAddClaims(context, IdentityModel.JwtClaimTypes.Address, result.Data.Address)
                .SafeAddClaims(context, "country", result.Data.Country)
                .SafeAddClaims(context, IdentityModel.JwtClaimTypes.Role, result.Data.IsGuide ? "guide" : "user");

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var exists = await UserService.ExistsAsync(context.Subject.GetSubjectId(), CancellationToken.None);
            context.IsActive = exists.Data;
        }

        private ProfileService SafeAddClaims(ProfileDataRequestContext context, string name, string value) {
            if(!string.IsNullOrEmpty(value))
            context.IssuedClaims.Add(new Claim(name, value));
            return this;
        }
    }
}
