using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Extensions
{
    public static class ProductMapping
    {
        // Chuyển từ ProductCreateVModel sang Entity Product
        public static Product ToEntity(this ProductCreateVModel model)
        {
            return new Product
            {
                ProductName = model.ProductName,
                SpecificationDescription = model.SpecificationDescription,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                CountriesId = model.CountriesId,
                SupplierId = model.SupplierId,
                IsActive = model.IsActive ?? true,
                CreatedDate = DateTime.UtcNow,
                ViewCount = 0
            };
        }

        // Cập nhật Entity Product từ ProductUpdateVModel
        public static void UpdateEntity(this Product entity, ProductUpdateVModel model)
        {
            entity.ProductName = model.ProductName;
            entity.SpecificationDescription = model.SpecificationDescription;
            entity.BrandId = model.BrandId;
            entity.CategoryId = model.CategoryId;
            entity.CountriesId = model.CountriesId;
            entity.SupplierId = model.SupplierId;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        // Chuyển từ Entity Product sang ProductGetVModel
        public static ProductGetVModel ToGetVModel(this Product entity)
        {
            return new ProductGetVModel
            {
                Id = entity.Id,
                ProductName = entity.ProductName,
                SpecificationDescription = entity.SpecificationDescription,
                BrandId = entity.BrandId,
                CategoryId = entity.CategoryId,
                CountriesId = entity.CountriesId,
                SupplierId = entity.SupplierId,
                IsActive = entity.IsActive,
                ViewCount = entity.ViewCount,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                BrandName = entity.Brand?.BrandName,
                CategoryName = entity.Category?.CategoryName,
                CountryName = entity.Countries?.CountryName,
                SupplierName = entity.Supplier?.SupplierName,
                ProductVariants = entity.ProductVariants?.Select(pv => new TomsFurnitureBackend.VModels.ProductVModel.ProductVariantGetVModel
                {
                    Id = pv.Id,
                    OriginalPrice = pv.OriginalPrice, // Bỏ ?? vì non-nullable
                    DiscountedPrice = pv.DiscountedPrice, // Bỏ ?? vì non-nullable
                    StockQty = pv.StockQty,
                    ImageUrl = pv.ImageUrl,
                    ColorName = pv.Color?.ColorName ?? string.Empty,
                    SizeName = pv.Size?.SizeName ?? string.Empty,
                    MaterialName = pv.Material?.MaterialName ?? string.Empty,
                    UnitName = pv.Unit?.UnitName ?? string.Empty
                }).ToList() ?? new List<TomsFurnitureBackend.VModels.ProductVModel.ProductVariantGetVModel>()
            };
        }
    }
}