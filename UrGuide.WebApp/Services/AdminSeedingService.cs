using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Services
{
    public interface IAdminSeedingService
    {
        Task SeedAdminUserAsync(string email, string password, string firstName, string lastName);
        Task SeedDefaultAdminAsync();
    }

    public class AdminSeedingService : IAdminSeedingService
    {
        private readonly UserManager<UrGuideUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminSeedingService> _logger;

        public AdminSeedingService(
            UserManager<UrGuideUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<AdminSeedingService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedDefaultAdminAsync()
        {
            // Only seed default admin if enabled in configuration
            var seedAdminEnabled = _configuration.GetValue<bool>("Seeding:SeedDefaultAdmin", false);
            
            if (!seedAdminEnabled)
            {
                _logger.LogInformation("Default admin seeding is disabled in configuration");
                return;
            }

            var adminEmail = _configuration.GetValue<string>("Seeding:AdminEmail") ?? "admin@urguide.local";
            var adminPassword = _configuration.GetValue<string>("Seeding:AdminPassword") ?? "Admin123!";
            var adminFirstName = _configuration.GetValue<string>("Seeding:AdminFirstName") ?? "Admin";
            var adminLastName = _configuration.GetValue<string>("Seeding:AdminLastName") ?? "User";

            await SeedAdminUserAsync(adminEmail, adminPassword, adminFirstName, adminLastName);
        }

        public async Task SeedAdminUserAsync(string email, string password, string firstName, string lastName)
        {
            try
            {
                // Check if Admin role exists, create if not
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    _logger.LogInformation("Creating Admin role");
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("Failed to create Admin role: {Errors}", 
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        return;
                    }
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    _logger.LogInformation("Admin user with email {Email} already exists", email);
                    
                    // Ensure user has Admin role
                    if (!await _userManager.IsInRoleAsync(existingUser, "Admin"))
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(existingUser, "Admin");
                        if (addRoleResult.Succeeded)
                        {
                            _logger.LogInformation("Added Admin role to existing user {Email}", email);
                        }
                    }
                    return;
                }

                // Create new admin user
                var adminUser = new UrGuideUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    IsGuide = false,
                    LockoutEnabled = false
                };

                var createResult = await _userManager.CreateAsync(adminUser, password);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create admin user: {Errors}", 
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return;
                }

                // Assign Admin role
                var roleAssignResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                if (!roleAssignResult.Succeeded)
                {
                    _logger.LogError("Failed to assign Admin role to user: {Errors}", 
                        string.Join(", ", roleAssignResult.Errors.Select(e => e.Description)));
                    return;
                }

                _logger.LogInformation("Successfully created admin user: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding admin user");
                throw;
            }
        }
    }
}
