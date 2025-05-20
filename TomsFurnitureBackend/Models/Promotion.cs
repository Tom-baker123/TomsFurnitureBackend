using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Promotion
{
    public int Id { get; set; }

    public string PromotionCode { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public decimal OrderMinimum { get; set; }

    public decimal MaximumDiscountAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int CouponUsage { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? PromotionTypeId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual PromotionType? PromotionType { get; set; }
}
