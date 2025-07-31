using System;

namespace TomsFurnitureBackend.VModels
{
    public class UserGuestCreateVModel
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string DetailAddress { get; set; } = null!;
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
    }

    public class UserGuestUpdateVModel : UserGuestCreateVModel
    {
        public int Id { get; set; }
    }

    public class UserGuestGetVModel : UserGuestUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
