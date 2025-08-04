namespace TomsFurnitureBackend.VModels
{
    /// <summary>
    /// ViewModel để tạo mới phản hồi
    /// </summary>
    public class FeedbackCreateVModel
    {
        /// <summary>
        /// Nội dung phản hồi, bắt buộc
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// ID phản hồi cha, tùy chọn
        /// </summary>
        public int? ParentFeedbackId { get; set; }

        /// <summary>
        /// Tên người dùng, bắt buộc nếu không đăng nhập
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Email, bắt buộc nếu không đăng nhập
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Số điện thoại, tùy chọn
        /// </summary>
        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// ViewModel để cập nhật phản hồi
    /// </summary>
    public class FeedbackUpdateVModel : FeedbackCreateVModel
    {
        /// <summary>
        /// ID của phản hồi
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Trạng thái hoạt động, tùy chọn
        /// </summary>
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// ViewModel để lấy thông tin phản hồi
    /// </summary>
    public class FeedbackGetVModel : FeedbackUpdateVModel
    {
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Danh sách ID phản hồi con
        /// </summary>
        public List<int> ChildFeedbackIds { get; set; } = new List<int>();

        /// <summary>
        /// ID người dùng, có thể null nếu không đăng nhập
        /// </summary>
        public int? UserId { get; set; }
    }
}