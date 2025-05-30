using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới xuất xứ
    public class CountryCreateVModel
    {
        // Tên xuất xứ, bắt buộc
        public string CountryName { get; set; } = null!;
    }

    // ViewModel để cập nhật xuất xứ
    public class CountryUpdateVModel : CountryCreateVModel
    {
        // Id của xuất xứ
        public int Id { get; set; }

        // Trạng thái hoạt động của xuất xứ
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin xuất xứ
    public class CountryGetVModel : CountryUpdateVModel
    {
        // URL ảnh của xuất xứ
        public string? ImageUrl { get; set; }

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