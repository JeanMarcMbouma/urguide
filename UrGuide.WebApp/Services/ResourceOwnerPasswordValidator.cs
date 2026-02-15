using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UrGuide.WebApp.Entities;
using Duende.IdentityModel;

namespace UrGuide.WebApp.Services
{
    /// <summary>
    /// Custom validator for Resource Owner Password Credentials grant
    /// Adds role and user claims to the access token
    /// </summary>
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<UrGuideUser> _userManager;
        private readonly SignInManager<UrGuideUser> _signInManager;

        public ResourceOwnerPasswordValidator(
            UserManager<UrGuideUser> userManager,
            SignInManager<UrGuideUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            // Find user by username or email
            var user = await _userManager.FindByNameAsync(context.UserName) 
                ?? await _userManager.FindByEmailAsync(context.UserName);

            if (user != null)
            {
                // Validate password
                var result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, lockoutOnFailure: true);
                
                if (result.Succeeded)
                {
                    // Get user roles
                    var roles = await _userManager.GetRolesAsync(user);
                    
                    // Build claims list including roles
                    var claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Subject, user.Id),
                        new Claim(JwtClaimTypes.Name, user.UserName ?? user.Email ?? ""),
                        new Claim(JwtClaimTypes.Email, user.Email ?? ""),
                        new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower(), ClaimValueTypes.Boolean)
                    };

                    // Add role claims
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(JwtClaimTypes.Role, role));
                    }

                    context.Result = new GrantValidationResult(
                        subject: user.Id,
                        authenticationMethod: "password",
                        claims: claims);

                    return;
                }
                else if (result.IsLockedOut)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Account locked out");
                    return;
                }
                else if (result.IsNotAllowed)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Account not allowed to sign in");
                    return;
                }
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
        }
    }
}
