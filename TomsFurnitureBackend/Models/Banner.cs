using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Banner
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string LinkUrl { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? DisplayOrder { get; set; }

    public string? Position { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
