using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Data
{
    public class UrGuideAuthContext : IdentityDbContext<UrGuideUser>
    {
        public UrGuideAuthContext(DbContextOptions<UrGuideAuthContext> options) : base(options)
        {
        }
    }
}
