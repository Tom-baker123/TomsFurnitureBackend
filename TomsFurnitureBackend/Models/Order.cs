using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal? Total { get; set; }

    public decimal PriceDiscount { get; set; }

    public decimal ShippingPrice { get; set; }

    public bool IsPaid { get; set; }

    public string? Note { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserId { get; set; }

    public int? OrderAddId { get; set; }

    public int? OrderStaId { get; set; }

    public int? PaymentMethodId { get; set; }

    public int? PromotionId { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public bool IsUserGuest { get; set; }

    public int? UserGuestId { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual OrderAddress? OrderAdd { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus? OrderSta { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual User? User { get; set; }

    public virtual UserGuest? UserGuest { get; set; }
}
