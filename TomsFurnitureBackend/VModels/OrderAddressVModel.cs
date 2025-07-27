using System.ComponentModel.DataAnnotations;

namespace TomsFurnitureBackend.VModels
{
    public class OrderAddressCreateVModel
    {
        [Required]
        public string Recipient { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string AddressDetailRecipient { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
        public int? CityCode { get; set; }
        [Required]
        public string District { get; set; } = null!;
        public int? DistrictCode { get; set; }
        [Required]
        public string Ward { get; set; } = null!;
        public int? WardCode { get; set; }
        public bool IsDeafaultAddress { get; set; }
        public int? UserId { get; set; }
    }

    public class OrderAddressUpdateVModel : OrderAddressCreateVModel
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; }
    }

    public class OrderAddressGetVModel : OrderAddressUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
