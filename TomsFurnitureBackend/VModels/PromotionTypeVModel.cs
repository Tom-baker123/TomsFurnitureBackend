using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới PromotionType
    public class PromotionTypeCreateVModel
    {
        // Tên loại khuyến mãi (bắt buộc)
        public string PromotionTypeName { get; set; } = null!;

        // Mô tả loại khuyến mãi
        public string? Description { get; set; }

        // Đơn vị khuyến mãi (0: %, 1: số tiền cố định, v.v.)
        public int PromotionUnit { get; set; }
    }

    // ViewModel để cập nhật PromotionType
    public class PromotionTypeUpdateVModel : PromotionTypeCreateVModel
    {
        // ID của PromotionType
        public int Id { get; set; }

        // Trạng thái hoạt động
        public bool? IsActive { get; set; }
    }

    // ViewModel để trả về thông tin PromotionType
    public class PromotionTypeGetVModel
    {
        // ID của PromotionType
        public int Id { get; set; }

        // Tên loại khuyến mãi
        public string PromotionTypeName { get; set; } = null!;

        // Mô tả
        public string? Description { get; set; }

        // Đơn vị khuyến mãi
        public int PromotionUnit { get; set; }

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
    }
}