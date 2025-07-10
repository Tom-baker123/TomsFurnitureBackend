using System.ComponentModel.DataAnnotations;

namespace TomsFurnitureBackend.VModels
{
    public class OrderStatusCreateVModel
    {
        [Required]
        public string OrderStatusName { get; set; } = null!;
    }

    public class OrderStatusUpdateVModel : OrderStatusCreateVModel
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; }
    }

    public class OrderStatusGetVModel : OrderStatusUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
