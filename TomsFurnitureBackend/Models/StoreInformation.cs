using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class StoreInformation
{
    public int Id { get; set; }

    public string? StoreName { get; set; }

    public string? StoreAddress { get; set; }

    public string? Logo { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? LinkWebsite { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? OwnerName { get; set; }

    public string BusinessType { get; set; } = null!;

    public string? OperatingHours { get; set; }

    public string? StoreDescription { get; set; }

    public DateOnly? EstablishmentDate { get; set; }

    public string? TaxId { get; set; }

    public string? BranchCode { get; set; }

    public string? LinkSocialFacebook { get; set; }

    public string? LinkSocialTwitter { get; set; }

    public string? LinkSocialInstagram { get; set; }

    public string? LinkSocialTiktok { get; set; }

    public string? LinkSocialYoutube { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
