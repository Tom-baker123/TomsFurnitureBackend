namespace TomsFurnitureBackend.Common.Models.Vnpay
{
    // Model ch?a thông tin tr? v? t? VNPAY sau khi thanh toán
    public class PaymentResponseModel
    {
        // Mô t? ??n hàng
        public string OrderDescription { get; set; }
        // Mã giao d?ch VNPAY
        public string TransactionId { get; set; }
        // Mã ??n hàng
        public string OrderId { get; set; }
        // Ph??ng th?c thanh toán
        public string PaymentMethod { get; set; }
        // Mã thanh toán
        public string PaymentId { get; set; }
        // Tr?ng thái thành công/th?t b?i
        public bool Success { get; set; }
        // Token xác th?c giao d?ch
        public string Token { get; set; }
        // Mã ph?n h?i t? VNPAY
        public string VnPayResponseCode { get; set; }
    }
}
