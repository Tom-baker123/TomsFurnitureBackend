namespace TomsFurnitureBackend.Common.Models.Vnpay
{
    // Model ch?a th�ng tin g?i l�n VNPAY khi t?o thanh to�n
    public class PaymentInformationModel
    {
        // Lo?i ??n h�ng
        public string OrderType { get; set; }
        // S? ti?n thanh to�n
        public double Amount { get; set; }
        // M� t? ??n h�ng
        public string OrderDescription { get; set; }
        // T�n ng??i ??t h�ng ho?c m� user
        public string Name { get; set; }
    }
}
