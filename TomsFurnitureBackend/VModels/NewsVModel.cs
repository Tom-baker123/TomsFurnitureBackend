using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới tin tức
    public class NewsCreateVModel
    {
        // Tiêu đề tin tức
        public string Title { get; set; } = null!;
        // Nội dung tin tức
        public string? Content { get; set; }
        // ID người tạo tin tức
        public int? UserId { get; set; }
    }

    // ViewModel để cập nhật tin tức
    public class NewsUpdateVModel : NewsCreateVModel
    {
        // ID của tin tức
        public int Id { get; set; }
        // Trạng thái hoạt động
        public bool? IsActive { get; set; }
    }

    // ViewModel để trả về thông tin tin tức
    public class NewsGetVModel : NewsUpdateVModel
    {
        // Đường dẫn ảnh đại diện
        public string? NewsAvatar { get; set; }
        // Ngày tạo
        public DateTime? CreatedDate { get; set; }
        // Ngày cập nhật
        public DateTime? UpdatedDate { get; set; }
        // Người tạo
        public string? CreatedBy { get; set; }
        // Người cập nhật
        public string? UpdatedBy { get; set; }
        // Tên người dùng tạo tin tức
        public string? UserName { get; set; }
    }
}