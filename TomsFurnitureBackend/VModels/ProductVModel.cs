using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TomsFurnitureBackend.VModels
{
    public class ProductVModel
    {
        public class ProductCreateVModel
        {
            
            public string ProductName { get; set; } = null!;


            public string? SpecificationDescription { get; set; }


            public int? BrandId { get; set; }


            public int? CategoryId { get; set; }


            public int? CountriesId { get; set; }


            public int? SupplierId { get; set; }


            public List<ProductVariantCreateVModel> ProductVariants { get; set; } = new List<ProductVariantCreateVModel>();
        }

        public class ProductUpdateVModel : ProductCreateVModel
        {
            public bool? IsActive { get; set; }

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

        public class ProductVariantCreateVModel
        {

            public decimal OriginalPrice { get; set; }


            public decimal? DiscountedPrice { get; set; }


            public int StockQty { get; set; }


            public string? ImageUrl { get; set; }


            public int ColorId { get; set; }


            public int SizeId { get; set; }


            public int MaterialId { get; set; }


            public int UnitId { get; set; }


            // public bool? IsActive { get; set; }
        }

        public class ProductVariantGetVModel
        {
            public int Id { get; set; }


            public decimal OriginalPrice { get; set; }


            public decimal? DiscountedPrice { get; set; }


            public int StockQty { get; set; }


            public string? ImageUrl { get; set; }


            public string? ColorName { get; set; }


            public string? SizeName { get; set; }


            public string? MaterialName { get; set; }


            public string? UnitName { get; set; }
        }
    }
}