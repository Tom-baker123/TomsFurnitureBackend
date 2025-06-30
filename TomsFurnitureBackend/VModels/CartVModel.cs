using System.ComponentModel.DataAnnotations;

namespace TomsFurnitureBackend.VModels
{
    public class CartVModel
    {
        public class CartCreateVModel
        {
            public int Quantity { get; set; }
            public int ProVarId { get; set; }
        }

        public class CartUpdateVModel : CartCreateVModel
        {
            public int Id { get; set; }
        }

        public class CartGetVModel
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string? CreatedBy { get; set; }
            public string? UpdatedBy { get; set; }
            public int? UserId { get; set; }
            public int ProVarId { get; set; }
            public string? ProductName { get; set; }
        }
    }
}