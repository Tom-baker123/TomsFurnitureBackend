using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới Promotion
    public class PromotionCreateVModel
    {
        // Mã giảm giá (bắt buộc, duy nhất)
        public string? PromotionCode { get; set; }

        // Giá trị giảm giá (%, số tiền, v.v.)
        public decimal DiscountValue { get; set; }

        // Giá trị đơn hàng tối thiểu để áp dụng
        public decimal OrderMinimum { get; set; }

        // Số tiền giảm tối đa
        public decimal MaximumDiscountAmount { get; set; }

        // Ngày bắt đầu
        public DateTime StartDate { get; set; }

        // Ngày kết thúc
        public DateTime EndDate { get; set; }

        // Số lần sử dụng tối đa
        public int CouponUsage { get; set; }

        // ID của loại khuyến mãi
        public int? PromotionTypeId { get; set; }
    }

    // ViewModel để cập nhật Promotion
    public class PromotionUpdateVModel : PromotionCreateVModel
    {
        // ID của Promotion
        public int Id { get; set; }

        // Trạng thái hoạt động
        public bool? IsActive { get; set; }
    }

    // ViewModel để trả về thông tin Promotion
    public class PromotionGetVModel : PromotionUpdateVModel
    {
        // Ngày tạo
        public DateTime? CreatedDate { get; set; }

        // Ngày cập nhật
        public DateTime? UpdatedDate { get; set; }

        // Người tạo
        public string? CreatedBy { get; set; }

        // Người cập nhật
        public string? UpdatedBy { get; set; }

        // Thông tin loại khuyến mãi
        public PromotionTypeGetVModel? PromotionType { get; set; }
    }
}