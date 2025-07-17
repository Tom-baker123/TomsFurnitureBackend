namespace TomsFurnitureBackend.Common.Models.Vnpay
{
    // Model ch?a th�ng tin tr? v? t? VNPAY sau khi thanh to�n
    public class PaymentResponseModel
    {
        // M� t? ??n h�ng
        public string OrderDescription { get; set; }
        // M� giao d?ch VNPAY
        public string TransactionId { get; set; }
        // M� ??n h�ng
        public string OrderId { get; set; }
        // Ph??ng th?c thanh to�n
        public string PaymentMethod { get; set; }
        // M� thanh to�n
        public string PaymentId { get; set; }
        // Tr?ng th�i th�nh c�ng/th?t b?i
        public bool Success { get; set; }
        // Token x�c th?c giao d?ch
        public string Token { get; set; }
        // M� ph?n h?i t? VNPAY
        public string VnPayResponseCode { get; set; }
    }
}
