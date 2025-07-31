namespace TomsFurnitureBackend.Common.Models.Vnpay
{
    public class PaymentInformationModel
    {
        public int OrderId { get; set; } // Th�m d�ng n�y
        public string OrderType { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string OrderDescription { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
