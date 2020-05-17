using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Posts;
using UrGuide.Model.Lookup;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;

namespace UrGuide.Services.Lookup
{
    class LookupService : ILookupService
    {
        public LookupService(UrGuideContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UrGuideContext Context { get; }

        public async Task<Result<IEnumerable<CategoryModel>>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var cats = await Context.Categories.AsNoTracking().ToListAsync(cancellationToken);
            var result = cats.Select(c => new CategoryModel
            {
                ImageUrl = c.ImageLink,
                Name = c.Name,
                Stats = Context.Posts.Count(p => p.Attributes.Any(a => EF.Functions.Like(a.Value, $"%{c.Name}%")
                && a.Name == nameof(AttributeTypes.Categories)))
            }).OrderByDescending(c => c.Stats).ThenBy(c => c.Name).AsEnumerable();

            return Result.Of(result);
        }
    }
}
