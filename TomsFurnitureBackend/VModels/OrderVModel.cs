using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.VModels
{
    public class OrderVModel
    {
        public class OrderCreateVModel
        {
            public int? OrderAddId { get; set; }
            public int? PaymentMethodId { get; set; }
            public int? PromotionId { get; set; }
            // public decimal? Total { get; set; }
            public decimal ShippingPrice { get; set; }
            public string? Note { get; set; }
            public List<OrderDetailCreateVModel> OrderDetails { get; set; } = new();
            public int? UserGuestId { get; set; }
            public string? PaymentStatus { get; set; }
        }

        public class OrderUpdateVModel : OrderCreateVModel
        {
            public int Id { get; set; }
            public bool? IsActive { get; set; }
            public int? OrderStaId { get; set; }
            public bool? IsPaid { get; set; }
        }

        public class OrderGetVModel : OrderUpdateVModel
        {
            public decimal? Total { get; set; }
            public int? UserId { get; set; }
            public bool IsUserGuest { get; set; }
            public int? UserGuestId { get; set; }
            public string OrderStatusName { get; set; } = null!;
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string? CreatedBy { get; set; }
            public string? UpdatedBy { get; set; }
            public List<OrderDetailGetVModel> OrderDetails { get; set; } = new();
            public string? PaymentStatus { get; set; }
        }

        public class OrderDetailCreateVModel
        {
            public int ProVarId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        public class OrderDetailGetVModel : OrderDetailCreateVModel
        {
            public int Id { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string? CreatedBy { get; set; }
            public string? UpdatedBy { get; set; }
            public ProductVModel.ProductVariantGetVModel? ProductVariant { get; set; }
        }
    }
}
