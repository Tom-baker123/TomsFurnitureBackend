namespace TomsFurnitureBackend.Common.Models
{
    // + "T": là kiểu dữ liệu tổng quát
    // + "where T : class": đảm bảo T phải là một class (reference type), không phải kiểu nguyên thủy(như int, bool).
    public class PaginationModel<T> where T : class
    {
        // Tổng số bản ghi (record) trong database
        public long TotalRecords { get; set; }
        // required để bắt buộc phải gán giá trị khi khởi tạo object. 
        // IEnumerable<T> cho phép duyệt qua danh sách (giống mảng, list...).
        public required IEnumerable<T> Records { get; set; }
    }
}
