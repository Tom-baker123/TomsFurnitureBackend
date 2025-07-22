namespace TomsFurnitureBackend.Common.Models.Vnpay
{
    public class PaymentInformationModel
    {
        public int OrderId { get; set; } // Thêm dòng này
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
    }
}
