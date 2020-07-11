using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core.Contracts;

namespace UrGuide.Core
{
    public class PagedList<T> : PagedList
    {
        public int PageNumber { get; set; } = 1;
        public int ItemsCount { get; internal set; }
        public IList<T> Items { get; internal set; }
    }

    public class PagedList
    {
        const int PageSize = 10;

        public async static Task<PagedList<T>> Of<T>(IQueryable<T> queryable, int pageNumber, CancellationToken cancellationToken)
        {
            var result = new PagedList<T>
            {
                ItemsCount = queryable.Count(),
                PageNumber = pageNumber
            };
            queryable = queryable.Skip((result.PageNumber - 1) * PageSize).Take(PageSize);
            result.Items = await queryable.ToListAsync(cancellationToken);
            return result;
        }

        public async static Task<PagedList<TResult>> Of<T, TResult>(IQueryable<T> queryable, int pageNumber, Func<T, TResult> map, CancellationToken cancellationToken)
        {
            var result = new PagedList<TResult>
            {
                ItemsCount = queryable.Count(),
                PageNumber = pageNumber
            };
            queryable = queryable.Skip((result.PageNumber - 1) * PageSize).Take(PageSize);
            var items = await queryable.ToListAsync(cancellationToken);
            result.Items = items.Select(i => map(i)).ToList();
            return result;
        }

        public static PagedList<TResult> Of<T, TResult>(IEnumerable<T> queryable, int pageNumber, Func<T, TResult> map)
        {
            var result = new PagedList<TResult>
            {
                ItemsCount = queryable.Count(),
                PageNumber = pageNumber
            };
            queryable = queryable.Skip((result.PageNumber - 1) * PageSize).Take(PageSize);
            var items = queryable.ToList();
            result.Items = items.Select(i => map(i)).ToList();
            return result;
        }

        public static PagedList<T> Of<T>(IEnumerable<T> queryable, int pageNumber)
        {
            var result = new PagedList<T>
            {
                ItemsCount = queryable.Count(),
                PageNumber = pageNumber
            };
            queryable = queryable.Skip((result.PageNumber - 1) * PageSize).Take(PageSize);
            result.Items = queryable.ToList();
            return result;
        }


    }

    public static class PagedListExtensions
    {
        public static PagedList<TResult> FromIdCollection<TResult>(this PagedList<string> pagedList, IQueryable<TResult> results) where TResult : IEntity
        {
            var ids = pagedList.Items;
            return new PagedList<TResult>
            {
                Items = results.Where(f => ids.Contains(f.Id)).ToList(),
                ItemsCount = pagedList.ItemsCount,
                PageNumber = pagedList.PageNumber
            };
        }

        public static PagedList<TResult> To<T, TResult>(this PagedList<T> pagedList, Func<T, TResult> map)
        {
            return new PagedList<TResult>
            {
                Items = pagedList.Items.Select(x => map(x)).ToList(),
                ItemsCount = pagedList.ItemsCount,
                PageNumber = pagedList.PageNumber
            };
        }
    }
}
