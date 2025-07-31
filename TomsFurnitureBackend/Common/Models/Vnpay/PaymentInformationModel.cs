namespace TomsFurnitureBackend.Common.Models.Vnpay
{
    public class PaymentInformationModel
    {
        public int OrderId { get; set; } // Thêm dòng này
        public string OrderType { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string OrderDescription { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
