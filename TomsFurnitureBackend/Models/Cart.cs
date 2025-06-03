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

    public int? ProId { get; set; }

    public virtual Product? Pro { get; set; }

    public virtual User? User { get; set; }
}
