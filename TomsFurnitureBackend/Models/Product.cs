using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Product
{
    public int Id { get; set; }

    public string ProductName { get; set; } = null!;

    public string? SpecificationDescription { get; set; }

    public int ViewCount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? BrandId { get; set; }

    public int? CategoryId { get; set; }

    public int? CountriesId { get; set; }

    public int? SupplierId { get; set; }

    public string? Slug { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Country? Countries { get; set; }

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual ICollection<Slider> Sliders { get; set; } = new List<Slider>();

    public virtual Supplier? Supplier { get; set; }
}
