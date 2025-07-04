using System.Text.Json.Serialization;
using TomsFurnitureBackend.Common.Contansts;

namespace TomsFurnitureBackend.VModels
{
    public class ProductVModel
    {
        public class ProductFilterParams
        {
            public string? Search { get; set; } // Tìm kiếm chung (giữ nguyên)
            public string? ProductName { get; set; } // Tìm kiếm theo tên sản phẩm
            public List<string>? CategoryNames { get; set; } // Lọc theo danh sách tên danh mục
            public List<string>? ColorNames { get; set; } // Lọc theo danh sách tên màu sắc
            public List<string>? MaterialNames { get; set; } // Lọc theo danh sách tên vật liệu
            public List<string>? SizeNames { get; set; } // Lọc theo danh sách tên kích thước
            public List<string>? BrandNames { get; set; } // Lọc theo danh sách tên thương hiệu
            public List<string>? CountryNames { get; set; } // Lọc theo danh sách tên xuất xứ
            public string? SortBy { get; set; } // Trường sắp xếp (ProductName, OriginalPrice, StockQty, CreatedDate, UpdatedDate)
            public string? SortOrder { get; set; } // Thứ tự sắp xếp (asc, desc)
            public int PageSize { get; set; } = Number.Pagination.DefaultPageSize;
            public int PageNumber { get; set; } = Number.Pagination.DefaultPageNumber;
            // Thêm thuộc tính MinPrice để lọc giá nhỏ nhất
            public decimal? MinPrice { get; set; }
            // Thêm thuộc tính MaxPrice để lọc giá lớn nhất
            public decimal? MaxPrice { get; set; }
            // Thêm thuộc tính InStock để lọc sản phẩm còn hàng (true) hoặc hết hàng (false)
            public bool? InStock { get; set; }
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
            public int Id { get; set; }
            // Sử dụng ProductVariantUpdateVModel để hỗ trợ cả tạo mới và cập nhật biến thể
            public new List<ProductVariantUpdateVModel> ProductVariants { get; set; } = new List<ProductVariantUpdateVModel>();
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
            public new List<ProductVariantGetVModel> ProductVariants { get; set; } = new List<ProductVariantGetVModel>();
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
            public int ColorId { get; set; }
            public string? ColorName { get; set; }
            public int SizeId { get; set; }
            public string? SizeName { get; set; }
            public int MaterialId { get; set; }
            public string? MaterialName { get; set; }
            public int UnitId { get; set; }
            public string? UnitName { get; set; }
        }
    }
}