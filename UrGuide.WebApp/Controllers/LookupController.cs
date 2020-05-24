using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Lookup;
using UrGuide.Services.Contracts;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LookupController : Controller
    {
        public LookupController(ILookupService lookupService)
        {
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        }

        public ILookupService LookupService { get; }

        [HttpGet("categories")]
        [ProducesDefaultResponseType(typeof(IEnumerable<CategoryModel>))]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await LookupService.GetCategoriesAsync(cancellationToken);
            return Ok(result.Data);
        }
    }
}
