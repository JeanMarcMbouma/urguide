using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.WebApp.Data;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Services
{
    public class SocialAuthService : ISocialAuthService
    {
        private readonly UrGuideAuthContext _context;
        private readonly UserManager<UrGuideUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<SocialAuthService> _logger;

        public SocialAuthService(
            UrGuideAuthContext context,
            UserManager<UrGuideUser> userManager,
            IJwtTokenService jwtTokenService,
            ILogger<SocialAuthService> logger)
        {
            _context = context;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        public async Task<SocialAuthResult> ProcessSocialLoginAsync(
            string provider,
            string providerKey,
            string? email,
            string? firstName,
            string? lastName,
            string? avatarUrl,
            CancellationToken cancellationToken)
        {
            // Check if there's already a linked social login for this provider + key
            var existingLink = await _context.SocialLoginProviders
                .Include(s => s.User)
                .FirstOrDefaultAsync(
                    s => s.Provider == provider && s.ProviderKey == providerKey,
                    cancellationToken);

            if (existingLink != null)
            {
                // Existing linked account — update last login and return token
                existingLink.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                var user = existingLink.User;
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtTokenService.GenerateToken(user.Id, user.UserName!, user.Email!, roles);

                _logger.LogInformation("Social login via {Provider} for existing user {UserId}", provider, user.Id);
                return SocialAuthResult.Ok(user.Id, token);
            }

            // No existing link — check if an account with the same email exists
            UrGuideUser? existingUser = null;
            if (!string.IsNullOrEmpty(email))
            {
                existingUser = await _userManager.FindByEmailAsync(email);
            }

            if (existingUser != null)
            {
                // Auto-link to existing account
                var link = new SocialLoginProvider
                {
                    UserId = existingUser.Id,
                    Provider = provider,
                    ProviderKey = providerKey,
                    Email = email,
                    DisplayName = $"{firstName} {lastName}".Trim(),
                    AvatarUrl = avatarUrl,
                    LinkedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                _context.SocialLoginProviders.Add(link);

                // Sync avatar if user doesn't have one
                if (string.IsNullOrEmpty(existingUser.AvatarUrl) && !string.IsNullOrEmpty(avatarUrl))
                {
                    existingUser.AvatarUrl = avatarUrl;
                }

                await _context.SaveChangesAsync(cancellationToken);

                var roles = await _userManager.GetRolesAsync(existingUser);
                var token = _jwtTokenService.GenerateToken(existingUser.Id, existingUser.UserName!, existingUser.Email!, roles);

                _logger.LogInformation("Social login via {Provider} auto-linked to existing user {UserId}", provider, existingUser.Id);
                return SocialAuthResult.Ok(existingUser.Id, token);
            }

            // Create a new account
            var newUser = new UrGuideUser
            {
                UserName = email ?? $"{provider}_{providerKey}",
                Email = email,
                EmailConfirmed = !string.IsNullOrEmpty(email), // Email from social provider is pre-verified
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                AvatarUrl = avatarUrl
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create user from social login {Provider}: {Errors}", provider, errors);
                return SocialAuthResult.Fail($"Account creation failed: {errors}");
            }

            var newLink = new SocialLoginProvider
            {
                UserId = newUser.Id,
                Provider = provider,
                ProviderKey = providerKey,
                Email = email,
                DisplayName = $"{firstName} {lastName}".Trim(),
                AvatarUrl = avatarUrl,
                LinkedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            _context.SocialLoginProviders.Add(newLink);
            await _context.SaveChangesAsync(cancellationToken);

            var newRoles = await _userManager.GetRolesAsync(newUser);
            var newToken = _jwtTokenService.GenerateToken(newUser.Id, newUser.UserName!, newUser.Email ?? "", newRoles);

            _logger.LogInformation("New user {UserId} created via social login {Provider}", newUser.Id, provider);
            return SocialAuthResult.Ok(newUser.Id, newToken, isNew: true);
        }

        public async Task<SocialAuthResult> LinkProviderAsync(
            string userId,
            string provider,
            string providerKey,
            string? email,
            string? displayName,
            string? avatarUrl,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return SocialAuthResult.Fail("User not found.");
            }

            // Check if the provider is already linked to this user
            var existingLink = await _context.SocialLoginProviders
                .FirstOrDefaultAsync(
                    s => s.UserId == userId && s.Provider == provider,
                    cancellationToken);

            if (existingLink != null)
            {
                return SocialAuthResult.Fail($"{provider} is already linked to your account.");
            }

            // Check if this provider key is already linked to another user
            var conflictLink = await _context.SocialLoginProviders
                .FirstOrDefaultAsync(
                    s => s.Provider == provider && s.ProviderKey == providerKey,
                    cancellationToken);

            if (conflictLink != null)
            {
                return SocialAuthResult.Fail($"This {provider} account is already linked to another user.");
            }

            var link = new SocialLoginProvider
            {
                UserId = userId,
                Provider = provider,
                ProviderKey = providerKey,
                Email = email,
                DisplayName = displayName,
                AvatarUrl = avatarUrl,
                LinkedAt = DateTime.UtcNow
            };

            _context.SocialLoginProviders.Add(link);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} linked {Provider} account", userId, provider);
            return SocialAuthResult.Ok(userId);
        }

        public async Task<SocialAuthResult> UnlinkProviderAsync(
            string userId,
            string provider,
            CancellationToken cancellationToken)
        {
            var link = await _context.SocialLoginProviders
                .FirstOrDefaultAsync(
                    s => s.UserId == userId && s.Provider == provider,
                    cancellationToken);

            if (link == null)
            {
                return SocialAuthResult.Fail($"{provider} is not linked to your account.");
            }

            // Ensure the user has another way to sign in (password or another provider)
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var hasPassword = await _userManager.HasPasswordAsync(user);
                var otherProviders = await _context.SocialLoginProviders
                    .CountAsync(s => s.UserId == userId && s.Provider != provider, cancellationToken);

                if (!hasPassword && otherProviders == 0)
                {
                    return SocialAuthResult.Fail(
                        "Cannot unlink the last login method. Set a password or link another provider first.");
                }
            }

            _context.SocialLoginProviders.Remove(link);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} unlinked {Provider} account", userId, provider);
            return SocialAuthResult.Ok(userId);
        }

        public async Task<IReadOnlyList<SocialLoginProviderDto>> GetLinkedProvidersAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            return await _context.SocialLoginProviders
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.Provider)
                .Select(s => new SocialLoginProviderDto
                {
                    Provider = s.Provider,
                    Email = s.Email,
                    DisplayName = s.DisplayName,
                    AvatarUrl = s.AvatarUrl,
                    LinkedAt = s.LinkedAt,
                    LastLoginAt = s.LastLoginAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SocialLoginAuditLogDto>> GetAuditLogAsync(
            string userId,
            int take,
            CancellationToken cancellationToken)
        {
            return await _context.SocialLoginAuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(take)
                .Select(a => new SocialLoginAuditLogDto
                {
                    Provider = a.Provider,
                    Action = a.Action,
                    Details = a.Details,
                    Timestamp = a.Timestamp
                })
                .ToListAsync(cancellationToken);
        }

        public async Task LogAuditEventAsync(
            string userId,
            string provider,
            string action,
            string? details,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken)
        {
            var log = new SocialLoginAuditLog
            {
                UserId = userId,
                Provider = provider,
                Action = action,
                Details = details,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            _context.SocialLoginAuditLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
