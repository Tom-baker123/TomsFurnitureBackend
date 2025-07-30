using System.Text.Json.Serialization;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới loại phòng
    public class RoomTypeCreateVModel
    {
        // Tên loại phòng
        public string RoomTypeName { get; set; } = null!;
        // Danh sách Id danh mục liên kết
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<int>? CategoryIds { get; set; }
    }

    // ViewModel để cập nhật loại phòng
    public class RoomTypeUpdateVModel : RoomTypeCreateVModel
    {
        // Id của loại phòng
        public int Id { get; set; }
        // Trạng thái hoạt động của loại phòng
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin loại phòng
    public class RoomTypeGetVModel : RoomTypeUpdateVModel
    {
        // Ảnh của loại phòng
        public string? ImageUrl { get; set; }
        // Slug của loại phòng
        public string? Slug { get; set; }
        // Ngày tạo
        public DateTime? CreatedDate { get; set; }
        // Ngày cập nhật
        public DateTime? UpdatedDate { get; set; }
        // Người tạo
        public string? CreatedBy { get; set; }
        // Người cập nhật
        public string? UpdatedBy { get; set; }
        // Danh sách danh mục liên kết (tóm tắt)
        public List<CategoryGetVModel>? Categories { get; set; }
    }
}
