using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới vai trò
    public class RoleCreateVModel
    {
        // Tên vai trò, bắt buộc
        public string RoleName { get; set; } = null!;
    }

    // ViewModel để cập nhật vai trò
    public class RoleUpdateVModel : RoleCreateVModel
    {
        // ID của vai trò
        public int Id { get; set; }

        // Trạng thái hoạt động, tùy chọn
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin vai trò
    public class RoleGetVModel : RoleUpdateVModel
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