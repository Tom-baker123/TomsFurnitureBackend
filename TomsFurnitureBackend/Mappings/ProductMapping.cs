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
            var product = new Product
            {
                ProductName = model.ProductName,
                SpecificationDescription = model.SpecificationDescription,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                CountriesId = model.CountriesId,
                SupplierId = model.SupplierId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ViewCount = 0,
                ProductVariants = model.ProductVariants.Select(pv => pv.ToEntity()).ToList()
            };
            return product;
        }

        // Chuyển từ ProductVariantCreateVModel sang Entity ProductVariant
        public static ProductVariant ToEntity(this ProductVariantCreateVModel model)
        {
            return new ProductVariant
            {
                OriginalPrice = model.OriginalPrice,
                DiscountedPrice = model.DiscountedPrice,
                StockQty = model.StockQty,
                ColorId = model.ColorId,
                SizeId = model.SizeId,
                MaterialId = model.MaterialId,
                UnitId = model.UnitId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
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
                ProductVariants = entity.ProductVariants?.Select(pv => new ProductVModel.ProductVariantGetVModel
                {
                    Id = pv.Id,
                    OriginalPrice = pv.OriginalPrice,
                    DiscountedPrice = pv.DiscountedPrice,
                    StockQty = pv.StockQty,
                    ColorName = pv.Color?.ColorName ?? string.Empty,
                    SizeName = pv.Size?.SizeName ?? string.Empty,
                    MaterialName = pv.Material?.MaterialName ?? string.Empty,
                    UnitName = pv.Unit?.UnitName ?? string.Empty
                }).ToList() ?? new List<ProductVModel.ProductVariantGetVModel>(),
                Sliders = entity.Sliders?.Where(s => s.IsActive == true).Select(s => new SliderGetVModel {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    LinkUrl = s.LinkUrl,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsPoster = s.IsPoster,
                    Position = s.Position,
                    DisplayOrder = s.DisplayOrder,
                    IsActive = s.IsActive,
                    CreatedDate = s.CreatedDate,
                    UpdatedDate = s.UpdatedDate,
                    CreatedBy = s.CreatedBy,
                    UpdatedBy = s.UpdatedBy
                }).ToList() ?? new List<SliderGetVModel>()
            };
        }
    }
}