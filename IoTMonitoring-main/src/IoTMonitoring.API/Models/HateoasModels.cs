namespace IoTMonitoring.API.Models
{
    public class HateoasLink
    {
        public string Href { get; set; } = string.Empty;
        public string Rel { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
    }

    public class HateoasResponse<T>
    {
        public T Data { get; set; }
        public List<HateoasLink> Links { get; set; } = new List<HateoasLink>();

        public HateoasResponse(T data)
        {
            Data = data;
        }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public List<HateoasLink> Links { get; set; } = new List<HateoasLink>();

        public PagedResult()
        {
        }

        public PagedResult(List<T> items, int count, int pageNumber, int pageSize, string? sortBy = null, bool sortDescending = false)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SortBy = sortBy;
            SortDescending = sortDescending;
        }
    }

    public class SearchParameters
    {
        public string? DeviceId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SensorType { get; set; }
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }
}
