using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Shipping
{
    public int Id { get; set; }

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public decimal ShippingPrice { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
