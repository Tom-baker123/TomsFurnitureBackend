using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class ProductVariant
{
    public int Id { get; set; }

    public decimal OriginalPrice { get; set; }

    public decimal? DiscountedPrice { get; set; }

    public int StockQty { get; set; }

    public int ProductId { get; set; }

    public int? ColorId { get; set; }

    public int? SizeId { get; set; }

    public int? MaterialId { get; set; }

    public int? UnitId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Color? Color { get; set; }

    public virtual Material? Material { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductVariantImage> ProductVariantImages { get; set; } = new List<ProductVariantImage>();

    public virtual Size? Size { get; set; }

    public virtual Unit? Unit { get; set; }
}
