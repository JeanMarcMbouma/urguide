using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UrGuide.WebApp.Services
{
    /// <summary>
    /// Service for generating JWT tokens for API authentication
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateToken(string userId, string userName, string email, IList<string> roles);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(string userId, string userName, string email, IList<string> roles)
        {
            var applicationUri = _configuration.GetValue<string>("ApplicationUri") 
                ?? _configuration.GetValue<string>("IdentityServer:ApplicationUri") 
                ?? "https://localhost:5001";

            // Use IdentityServer's signing key or a custom JWT key
            var jwtKey = _configuration.GetValue<string>("Jwt:Key");
            
            // If no custom JWT key is configured, generate a development key based on application URI
            // In production, this should be a secure, randomly generated key stored in secrets
            if (string.IsNullOrEmpty(jwtKey))
            {
                jwtKey = $"UrGuide_JWT_Secret_Key_{applicationUri}_Development_Only";
                _logger.LogWarning("No JWT key configured. Using generated development key. Configure 'Jwt:Key' in production.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var expiresInHours = _configuration.GetValue<int?>("Jwt:ExpiresInHours") ?? 8;

            var token = new JwtSecurityToken(
                issuer: applicationUri,
                audience: applicationUri,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiresInHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
