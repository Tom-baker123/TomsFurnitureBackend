namespace WebPizza_API_BackEnd.Common.Models
{
    public class PaginationModel<T> where T : class
    {
        public long TotalRecords { get; set; }
        public required IEnumerable<T> Records { get; set; }
    }
}
