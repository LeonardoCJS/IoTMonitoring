namespace IoTMonitoring.Web.ViewModels
{
    public class PaginatedViewModel<T> where T : class
    {
        public List Items { get; set; } = new List();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public string SortBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; }

        public PaginatedViewModel()
        {
        }

        public PaginatedViewModel(List items, int count, int pageNumber, int pageSize, string sortBy = "", bool sortDescending = false)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SortBy = sortBy;
            SortDescending = sortDescending;
        }

        public static PaginatedViewModel<T> Create(IQueryable<T> source, int pageNumber, int pageSize, string sortBy = "", bool sortDescending = false)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedViewModel<T>(items, count, pageNumber, pageSize, sortBy, sortDescending);
        }
    }

    public class HomeViewModel
    {
        public int TotalDevices { get; set; }
        public int OnlineDevices { get; set; }
        public int OfflineDevices { get; set; }
        public int ErrorDevices { get; set; }
        public int TotalSensorReadings { get; set; }
        public List RecentDevices { get; set; } = new List();
        public List RecentSensorData { get; set; } = new List();
    }
}
