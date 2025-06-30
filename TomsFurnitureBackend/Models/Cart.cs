using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserId { get; set; }

    public int ProVarId { get; set; }

    public virtual ProductVariant ProVar { get; set; } = null!;

    public virtual User? User { get; set; }
}
