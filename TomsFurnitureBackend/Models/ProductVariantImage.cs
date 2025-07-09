using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class ProductVariantImage
{
    public int Id { get; set; }

    public string? ImageUrl { get; set; }

    public string? Attribute { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProVarId { get; set; }

    public virtual ProductVariant? ProVar { get; set; }
}
