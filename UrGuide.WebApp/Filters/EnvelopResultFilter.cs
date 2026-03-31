using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using UrGuide.WebApp.Models;
using UrGuide.WebApp.Resources;

namespace UrGuide.WebApp.Filters
{
    public class EnvelopResultFilter : IAsyncAlwaysRunResultFilter
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public EnvelopResultFilter(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if(context.Controller is Controller ct && !ct.ModelState.IsValid)
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(ErrorEnvelop.Create(ct.ModelState));
                await context.Result.ExecuteResultAsync(context);
                context.Cancel = true;
            } 
            else if(context.Result is StatusCodeResult sc && sc.StatusCode == 500)
            {
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new JsonResult(ErrorEnvelop.Create(new[] { _localizer["Error_InternalServer"].Value }));
                await context.Result.ExecuteResultAsync(context);
                context.Cancel = true;
            }
            else
            {
              await next();
            }
        }
    }
}
