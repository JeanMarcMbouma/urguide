using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrGuide.Model.Catalogs;
using UrGuide.Model.Shared;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{

    [ApiController]
    [Route("catalogs")]
    [Authorize]
    public class GalleriesController : ControllerBase
    {

        public ICatalogService CatalogService { get; }
        public IUserContext UserContext { get; }

        public GalleriesController(ICatalogService catalogService, IUserContext userContext)
        {
            CatalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            UserContext = userContext;
        }

        // GET: api/Gallery
        [HttpGet("{userId}/all")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<ImageCatalogModel>))]

        public async Task<IActionResult> Get(string userId, CancellationToken cancellationToken)
        {
            var result = await CatalogService.GetCatalogsAsync(userId ?? UserContext.UserId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("{catalogId}/retrieve")]
        [ProducesDefaultResponseType(typeof(ImageCatalogModel))]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string catalogId, CancellationToken cancellationToken)
        {
            var result = await CatalogService.GetCatalogAsync(catalogId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        // PUT: api/Gallery/5
        [HttpPut("{catalogId}/update")]
        public async Task<IActionResult> Update(string catalogId, [FromBody]Model.Catalogs.UpdateImageCatalogModel model, CancellationToken cancellationToken)
        {
            if (catalogId != model.CatalogId)
                return BadRequest();

            var result = await CatalogService.SetCataglogAttributesAsync(model.CatalogId, new[] {
                new Model.SetAttribute { Name = nameof(model.Name), Value = model.Name },
                new Model.SetAttribute { Name = nameof(model.Description), Value = model.Description }
            }, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPut("update/{catalogId}/images/{imageId}/remove")]
        public async Task<IActionResult> RemoveImage(string catalogId, string imageId, CancellationToken cancellationToken)
        {
            var result = await CatalogService.RemoveImageFromCatalogAsync(catalogId, new[] {
                imageId 
            }, cancellationToken);
            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));
            return await GetById(catalogId, cancellationToken);
        }

        [HttpPut("update/{catalogId}/addimage")]
        public async Task<IActionResult> AddImage(string catalogId, [FromBody]ImageFileCreateModel imageFile, CancellationToken cancellationToken)
        {
            var result = await CatalogService.AddImageToCatalogAsync(catalogId, imageFile, cancellationToken);
            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));
            return await GetById(catalogId, cancellationToken);
        }

        // POST: api/Galleries
        [HttpPost("create")]
        [ProducesDefaultResponseType(typeof(ImageCatalogModel))]
        public async Task<IActionResult> Create([FromBody]Model.Catalogs.CreateImageCatalogModel model, CancellationToken cancellationToken)
        {
            var result = await CatalogService.CreateCatalogAsync(model, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        // DELETE: api/Galleries/5
        [HttpDelete("{catalogId}/remove")]
        public async Task<IActionResult> Delete(string catalogId, CancellationToken cancellationToken)
        {
            var result = await CatalogService.RemoveCatalogAsync(catalogId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }
    }
}
