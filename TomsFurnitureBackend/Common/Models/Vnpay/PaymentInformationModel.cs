namespace TomsFurnitureBackend.Common.Models.Vnpay
{
    public class PaymentInformationModel
    {
        public int OrderId { get; set; } // Th�m d�ng n�y
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
    }
}
