using System.ComponentModel.DataAnnotations;

namespace TomsFurnitureBackend.VModels
{
    public class CartVModel
    {
        public class CartCreateVModel
        {
            [Required(ErrorMessage = "Số lượng là bắt buộc.")]
            public int Quantity { get; set; }

            [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
            public int ProId { get; set; }
        }

        public class CartUpdateVModel : CartCreateVModel
        {
            [Required(ErrorMessage = "ID giỏ hàng là bắt buộc.")]
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
            public int? ProId { get; set; }
            public string? ProductName { get; set; }
        }
    }
}