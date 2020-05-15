using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if(result.Data.IsGuide) {
               context.IssuedClaims.AddRange(new[]
                {
                    new Claim(IdentityModel.JwtClaimTypes.BirthDate, result.Data.BirthDay),
                    new Claim(IdentityModel.JwtClaimTypes.Picture, result.Data.ProfileImage),
                    new Claim(IdentityModel.JwtClaimTypes.FamilyName, result.Data.LastName),
                    new Claim(IdentityModel.JwtClaimTypes.GivenName, result.Data.FirstName),
                    new Claim(IdentityModel.JwtClaimTypes.Gender, result.Data.Gender),
                    new Claim(IdentityModel.JwtClaimTypes.Name, result.Data.FullName),
                    new Claim(IdentityModel.JwtClaimTypes.Address, result.Data.Address),
                    new Claim("country", result.Data.Country),
                    new Claim(IdentityModel.JwtClaimTypes.Role, result.Data.IsGuide ? "guide" : "user")
                });
            } else {
                context.IssuedClaims.AddRange(new[]
                {
                    new Claim(IdentityModel.JwtClaimTypes.FamilyName, result.Data.LastName),
                    new Claim(IdentityModel.JwtClaimTypes.GivenName, result.Data.FirstName),
                    new Claim(IdentityModel.JwtClaimTypes.Name, result.Data.FullName),
                    new Claim(IdentityModel.JwtClaimTypes.Role, result.Data.IsGuide ? "guide" : "user")
                });
            }
            
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var exists = await UserService.ExistsAsync(context.Subject.GetSubjectId(), CancellationToken.None);
            context.IsActive = exists.Data;
        }
    }
}
