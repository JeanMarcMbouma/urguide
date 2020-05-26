using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Filters
{
    public class EnvelopResultFilter : IAsyncAlwaysRunResultFilter
    {
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
                context.Result = new JsonResult(ErrorEnvelop.Create(new[] { "Sorry, an unexpected error has occured." }));
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
