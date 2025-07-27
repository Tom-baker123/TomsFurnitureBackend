using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public string? Descriptions { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ParentId { get; set; }

    public int? RoomTypeId { get; set; }

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category? Parent { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual RoomType? RoomType { get; set; }
}
