using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Supplier
{
    public int Id { get; set; }

    public string? SupplierName { get; set; }

    public string? ContactName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? ImageUrl { get; set; }

    public string? Notes { get; set; }

    public string TaxId { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
