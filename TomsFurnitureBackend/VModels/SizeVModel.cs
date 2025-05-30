using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới kích thước
    public class SizeCreateVModel
    {
        // Tên kích thước, bắt buộc
        public string SizeName { get; set; } = null!;
    }

    // ViewModel để cập nhật kích thước
    public class SizeUpdateVModel : SizeCreateVModel
    {
        // Id của kích thước
        public int Id { get; set; }

        // Trạng thái hoạt động của kích thước
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin kích thước
    public class SizeGetVModel : SizeUpdateVModel
    {
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