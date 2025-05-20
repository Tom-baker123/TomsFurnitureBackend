using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class OrderStatus
{
    public int Id { get; set; }

    public string OrderStatusName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
