using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Data
{
    public class UrGuideAuthContext : ApiAuthorizationDbContext<UrGuideUser>
    {
        public UrGuideAuthContext(DbContextOptions<UrGuideAuthContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
    }
}
