using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TomsFurnitureBackend.VModels
{
    public class ProductVModel
    {
        public class ProductCreateVModel
        {
            [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
            [MaxLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
            public string ProductName { get; set; } = null!;

            [MaxLength(1000, ErrorMessage = "Mô tả đặc điểm kỹ thuật không được vượt quá 1000 ký tự.")]
            public string? SpecificationDescription { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "ID thương hiệu phải là một số nguyên dương.")]
            [DefaultValue(0)]
            public int? BrandId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "ID danh mục phải là một số nguyên dương.")]
            [DefaultValue(0)]
            public int? CategoryId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "ID quốc gia phải là một số nguyên dương.")]
            [DefaultValue(0)]
            public int? CountriesId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "ID nhà cung cấp phải là một số nguyên dương.")]
            [DefaultValue(0)]
            public int? SupplierId { get; set; }

            [DefaultValue(true)]
            public bool? IsActive { get; set; }
        }

        public class ProductUpdateVModel : ProductCreateVModel
        {
            [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
            [Range(1, int.MaxValue, ErrorMessage = "ID sản phẩm phải là một số nguyên dương.")]
            [DefaultValue(0)]
            public int Id { get; set; }
        }

        public class ProductGetVModel : ProductUpdateVModel
        {
            public int ViewCount { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string? CreatedBy { get; set; }
            public string? UpdatedBy { get; set; }
            public string? BrandName { get; set; }
            public string? CategoryName { get; set; }
            public string? CountryName { get; set; }
            public string? SupplierName { get; set; }
            public List<ProductVariantGetVModel> ProductVariants { get; set; } = new List<ProductVariantGetVModel>();
        }

        public class ProductVariantGetVModel
        {
            public int Id { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải là một số không âm.")]
            public decimal? OriginalPrice { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Giá giảm phải là một số không âm.")]
            public decimal? DiscountedPrice { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải là một số không âm.")]
            public int StockQty { get; set; }

            [MaxLength(500, ErrorMessage = "URL ảnh không được vượt quá 500 ký tự.")]
            public string? ImageUrl { get; set; }

            [MaxLength(100, ErrorMessage = "Tên màu không được vượt quá 100 ký tự.")]
            public string? ColorName { get; set; }

            [MaxLength(100, ErrorMessage = "Tên kích thước không được vượt quá 100 ký tự.")]
            public string? SizeName { get; set; }

            [MaxLength(100, ErrorMessage = "Tên chất liệu không được vượt quá 100 ký tự.")]
            public string? MaterialName { get; set; }

            [MaxLength(100, ErrorMessage = "Tên đơn vị không được vượt quá 100 ký tự.")]
            public string? UnitName { get; set; }
        }
    }
}