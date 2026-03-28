using Microsoft.EntityFrameworkCore;
using System;
using BbQ.Outcome;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Model.Lookup;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;

namespace UrGuide.Services.Lookup
{
    public class LookupService : ILookupService
    {
        public LookupService(UrGuideContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UrGuideContext Context { get; }

        public async Task<Outcome<IEnumerable<CategoryModel>>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var cats = await Context.Categories.AsNoTracking().ToListAsync(cancellationToken);
            var result = cats.Select(c => new CategoryModel
            {
                ImageUrl = c.ImageLink,
                Name = c.Name,
                Stats = Context.Posts.Count(p => EF.Functions.Like(p.Tags, $"%{c.Name}%"))
            }).OrderByDescending(c => c.Stats).ThenBy(c => c.Name).AsEnumerable();

            return Result.Of(result);
        }

        public async Task<Outcome<IEnumerable<RegionModel>>> GetRegionsAsync(CancellationToken cancellationToken)
        {
            var regions = await Context.Regions.AsNoTracking().ToListAsync(cancellationToken);
            var result = regions.Select(r => new RegionModel
            {
                RegionId = r.RegionId,
                Name = r.Name,
                CurrencyId = r.CurrencyId
            }).OrderBy(r => r.Name).AsEnumerable();

            return Result.Of(result);
        }
    }
}
