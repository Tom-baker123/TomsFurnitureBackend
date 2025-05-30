using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới nhà cung cấp
    public class SupplierCreateVModel
    {
        // Tên nhà cung cấp, không bắt buộc
        public string? SupplierName { get; set; }

        // Tên người liên hệ, không bắt buộc
        public string? ContactName { get; set; }

        // Email, bắt buộc
        public string Email { get; set; } = null!;

        // Số điện thoại, không bắt buộc
        public string? PhoneNumber { get; set; }

        // Ghi chú, không bắt buộc
        public string? Notes { get; set; }

        // Mã số thuế, bắt buộc
        public string TaxId { get; set; } = null!;
    }

    // ViewModel để cập nhật nhà cung cấp
    public class SupplierUpdateVModel : SupplierCreateVModel
    {
        // Id của nhà cung cấp
        public int Id { get; set; }

        // Trạng thái hoạt động của nhà cung cấp
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin nhà cung cấp
    public class SupplierGetVModel : SupplierUpdateVModel
    {
        // URL ảnh của nhà cung cấp
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