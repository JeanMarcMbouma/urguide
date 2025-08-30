using System.Collections.Generic;

namespace UrGuide.Model.Results;

public class PageResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; } = 0;
    public bool HasNextPage { get; set; } = false;
    public bool HasPreviousPage { get; set; } = false;
}