using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using UrGuide.Shared.Contracts;

namespace UrGuide.WebApp.Filters
{
    public class SaveChangeActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var webHelper = (Services.WebHelper)context.HttpContext.RequestServices.GetRequiredService<IWebHelper>();
            var result = await next();
            if (result.Exception == null || result.ExceptionHandled)
            {
                DbContext dbContext = context.HttpContext.RequestServices.GetService<Data.UrGuideAuthContext>();
                await dbContext.SaveChangesAsync();
                dbContext = context.HttpContext.RequestServices.GetService<UrGuide.Data.UrGuideContext>();
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
