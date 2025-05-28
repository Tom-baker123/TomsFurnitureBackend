namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới vật liệu
    public class MaterialCreateVModel
    {
        // Tên vật liệu, bắt buộc
        public string MaterialName { get; set; } = null!;
    }

    // ViewModel để cập nhật vật liệu, kế thừa từ CreateVModel và thêm Id, IsActive
    public class MaterialUpdateVModel : MaterialCreateVModel {
        // Id của vật liệu
        public int Id { get; set; }
        // Trạng thái hoạt động
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin vật liệu, bao gồm các trường bổ sung
    public class MaterialGetVModel : MaterialUpdateVModel {
        // Ngày tạo
        public DateTime? CreatedDate { get; set; }
        // Ngày cập nhật
        public DateTime? UpdatedDate { get; set; }
        // Người tạo
        public string? CreatedBy { get; set; }
        // Người cập nhật
        public string? UpdatedBy { get; set; }
    }
}
