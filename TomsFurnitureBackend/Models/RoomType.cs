using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class RoomType
{
    public int Id { get; set; }

    public string RoomTypeName { get; set; } = null!;

    public string? Slug { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
