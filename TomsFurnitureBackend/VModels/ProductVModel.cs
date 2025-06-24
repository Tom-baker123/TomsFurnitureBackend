using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TomsFurnitureBackend.Common.Contansts; // Add this namespace

namespace TomsFurnitureBackend.VModels
{
    public class ProductVModel
    {
        // VModel cho Phân Trang
        public class ProductFilterParams
        {
            public string? search { get; set; }
            public int PageSize { get; set; } = Number.Pagination.DefaultPageSize;
            public int PageNumber { get; set; } = Number.Pagination.DefaultPageNumber;
        }

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
            [JsonPropertyOrder(-1)] // Negative value ensures Id appears first
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
            public List<SliderGetVModel> Sliders { get; set; } = new List<SliderGetVModel>();
        }

        public class ProductVariantCreateVModel
        {

            public decimal OriginalPrice { get; set; }


            public decimal? DiscountedPrice { get; set; }


            public int StockQty { get; set; }


            public int ColorId { get; set; }


            public int SizeId { get; set; }


            public int MaterialId { get; set; }


            public int UnitId { get; set; }


            // public bool? IsActive { get; set; }
        }

        public class ProductVariantUpdateVModel
        {
            [JsonPropertyOrder(-1)]
            public int Id { get; set; }

            public decimal OriginalPrice { get; set; }

            public decimal? DiscountedPrice { get; set; }

            public int StockQty { get; set; }

            public int ColorId { get; set; }

            public int SizeId { get; set; }

            public int MaterialId { get; set; }

            public int UnitId { get; set; }

            public bool? IsActive { get; set; }
        }

        public class ProductVariantGetVModel
        {
            public int Id { get; set; }


            public decimal OriginalPrice { get; set; }


            public decimal? DiscountedPrice { get; set; }


            public int StockQty { get; set; }


            public int ColorId { get; set; } // Thay ColorName thành ColorId
            public string? ColorName { get; set; }
            public int SizeId { get; set; } // Thay SizeName thành SizeId
            public string? SizeName { get; set; }
            public int MaterialId { get; set; } // Thay MaterialName thành MaterialId
            public string? MaterialName { get; set; }
            public int UnitId { get; set; } // Thay UnitName thành UnitId
            public string? UnitName { get; set; }
        }
    }
}