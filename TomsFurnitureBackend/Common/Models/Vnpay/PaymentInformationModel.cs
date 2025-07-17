namespace TomsFurnitureBackend.Common.Models.Vnpay
{
    // Model ch?a thông tin g?i lên VNPAY khi t?o thanh toán
    public class PaymentInformationModel
    {
        // Lo?i ??n hàng
        public string OrderType { get; set; }
        // S? ti?n thanh toán
        public double Amount { get; set; }
        // Mô t? ??n hàng
        public string OrderDescription { get; set; }
        // Tên ng??i ??t hàng ho?c mã user
        public string Name { get; set; }
    }
}
