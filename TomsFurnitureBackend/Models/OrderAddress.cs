using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class OrderAddress
{
    public int Id { get; set; }

    public string Recipient { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string AddressDetailRecipient { get; set; } = null!;

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public bool IsDeafaultAddress { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserId { get; set; }

    public int? CityCode { get; set; }

    public int? DistrictCode { get; set; }

    public int? WardCode { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; }
}
