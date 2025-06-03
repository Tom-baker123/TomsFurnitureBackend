using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class UserGuest
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string DetailAddress { get; set; } = null!;

    public string? City { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
