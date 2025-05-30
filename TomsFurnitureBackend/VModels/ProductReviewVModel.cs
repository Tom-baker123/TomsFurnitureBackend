using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel cho Comment
    public class CommentVModel
    {
        // Nội dung bình luận, không bắt buộc
        public string? Content { get; set; }

        // Số lượt thích, mặc định 0
        public int LikeCount { get; set; }

        // ID người dùng, bắt buộc
        public int UserId { get; set; }
    }

    // ViewModel cho ImageProductReview
    public class ImageProductReviewVModel
    {
        // Thuộc tính ảnh, không bắt buộc
        public string? Attribute { get; set; }
    }

    // ViewModel để tạo mới đánh giá sản phẩm
    public class ProductReviewCreateVModel
    {
        // Điểm đánh giá (1-5), bắt buộc
        public int StarRating { get; set; }

        // Nội dung đánh giá, không bắt buộc
        public string? Content { get; set; }

        // ID người dùng, bắt buộc
        public int UserId { get; set; }

        // ID sản phẩm, bắt buộc
        public int ProId { get; set; }

        // Danh sách bình luận
        public List<CommentVModel> Comments { get; set; } = new List<CommentVModel>();

        // Danh sách ảnh đánh giá
        public List<ImageProductReviewVModel> ImageReviews { get; set; } = new List<ImageProductReviewVModel>();
    }

    // ViewModel để cập nhật đánh giá sản phẩm
    public class ProductReviewUpdateVModel : ProductReviewCreateVModel
    {
        // ID của đánh giá
        public int Id { get; set; }

        // Trạng thái hoạt động
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin đánh giá sản phẩm
    public class ProductReviewGetVModel : ProductReviewUpdateVModel
    {
        // URL video liên quan
        public string? RelatedVideo { get; set; }

        // Ngày tạo
        public DateTime? CreatedDate { get; set; }

        // Ngày cập nhật
        public DateTime? UpdatedDate { get; set; }

        // Người tạo
        public string? CreatedBy { get; set; }

        // Người cập nhật
        public string? UpdatedBy { get; set; }

        // Danh sách URL ảnh
        public List<string> ImageUrls { get; set; } = new List<string>();

        // Danh sách bình luận đầy đủ
        public List<CommentGetVModel> CommentDetails { get; set; } = new List<CommentGetVModel>();
    }

    // ViewModel để lấy thông tin bình luận
    public class CommentGetVModel
    {
        // ID bình luận
        public int Id { get; set; }

        // Nội dung bình luận
        public string? Content { get; set; }

        // Số lượt thích
        public int LikeCount { get; set; }

        // Trạng thái hoạt động
        public bool? IsActive { get; set; }

        // Ngày tạo
        public DateTime? CreatedDate { get; set; }

        // Ngày cập nhật
        public DateTime? UpdatedDate { get; set; }

        // Người tạo
        public string? CreatedBy { get; set; }

        // Người cập nhật
        public string? UpdatedBy { get; set; }

        // ID người dùng
        public int UserId { get; set; }
    }
}