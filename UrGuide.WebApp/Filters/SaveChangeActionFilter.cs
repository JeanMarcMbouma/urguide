using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Filters
{
    public class SaveChangeActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();
            if (result.Exception == null || result.ExceptionHandled)
            {
                var authContext = context.HttpContext.RequestServices.GetService<Data.UrGuideAuthContext>();
                if (authContext != null)
                {
                    await authContext.SaveChangesAsync();
                }

                var dataContext = context.HttpContext.RequestServices.GetService<UrGuide.Data.UrGuideContext>();
                if (dataContext != null)
                {
                    await dataContext.SaveChangesAsync();
                }
            }
        }
    }
}
