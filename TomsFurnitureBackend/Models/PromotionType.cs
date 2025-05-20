using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class PromotionType
{
    public int Id { get; set; }

    public string PromotionTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public int PromotionUnit { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}
