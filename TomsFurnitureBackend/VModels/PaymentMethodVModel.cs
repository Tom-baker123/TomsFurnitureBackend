using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới phương thức thanh toán
    public class PaymentMethodCreateVModel
    {
        // Tên phương thức thanh toán, bắt buộc
        public string NamePaymentMethod { get; set; } = null!;
    }

    // ViewModel để cập nhật phương thức thanh toán
    public class PaymentMethodUpdateVModel : PaymentMethodCreateVModel
    {
        // ID của phương thức thanh toán
        public int Id { get; set; }

        // Trạng thái hoạt động, tùy chọn
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin phương thức thanh toán
    public class PaymentMethodGetVModel : PaymentMethodUpdateVModel
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