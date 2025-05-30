using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới phản hồi
    public class FeedbackCreateVModel
    {
        // Nội dung phản hồi, bắt buộc
        public string Message { get; set; } = null!;

        // ID phản hồi cha, tùy chọn
        public int? ParentFeedbackId { get; set; }

        // ID người dùng, bắt buộc
        public int UserId { get; set; }
    }

    // ViewModel để cập nhật phản hồi
    public class FeedbackUpdateVModel : FeedbackCreateVModel
    {
        // ID của phản hồi
        public int Id { get; set; }

        // Trạng thái hoạt động, tùy chọn
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin phản hồi
    public class FeedbackGetVModel : FeedbackUpdateVModel
    {
        // Ngày tạo
        public DateTime? CreatedDate { get; set; }

        // Ngày cập nhật
        public DateTime? UpdatedDate { get; set; }

        // Người tạo
        public string? CreatedBy { get; set; }

        // Người cập nhật
        public string? UpdatedBy { get; set; }

        // Danh sách ID phản hồi con
        public List<int> ChildFeedbackIds { get; set; } = new List<int>();
    }
}